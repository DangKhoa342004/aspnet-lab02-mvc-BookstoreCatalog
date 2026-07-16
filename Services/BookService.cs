using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;
using BookstoreCatalog.Mvc.Repositories;
using BookstoreCatalog.Mvc.Options;
using BookstoreCatalog.Mvc.Data;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging; 
using Microsoft.EntityFrameworkCore;

namespace BookstoreCatalog.Mvc.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly AppSettings _settings;
    private readonly AppDbContext _context;
    private readonly ILogger<BookService> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileUploadService _fileUploadService;

    public BookService(IBookRepository bookRepository, IOptions<AppSettings> options, AppDbContext context, 
            ILogger<BookService> logger, IWebHostEnvironment webHostEnvironment, IFileUploadService fileUploadService)
    {
        _bookRepository = bookRepository;
        _settings = options.Value;
        _context = context;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        _fileUploadService = fileUploadService;
    }

    public async Task<List<BookListViewModel>> GetBookListAsync()
    {
        var books = await _bookRepository.GetAllReadOnlyAsync();
        return books.Select(b => new BookListViewModel
        {
            Id = b.Id,
            ISBN = b.ISBN,
            Title = b.Title,
            Author = b.Author,
            Price = b.Price,
            Quantity = b.Quantity,
            GenreName = b.Genre != null ? b.Genre.Name : "N/A",
            ImagePath = b.ImagePath,
            IsLowStock = b.Quantity < _settings.LowStockThreshold && b.Quantity > 0
        }).ToList();
    }

    public async Task<BookDetailViewModel?> GetBookDetailAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            _logger.LogError("Invalid request: BookId={BookId}", id);
            await _context.SaveChangesAsync();
            return null;
        }

        return new BookDetailViewModel
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            Quantity = book.Quantity,
            MinStock = book.MinStock,
            GenreName = book.Genre != null ? book.Genre.Name : "N/A",
            ImagePath = book.ImagePath,
            IsLowStock = book.Quantity < _settings.LowStockThreshold && book.Quantity > 0
        };
    }

    public async Task<BookFilterViewModel> GetFilteredBooksViewModelAsync(string? keyword, int? genreId, decimal? minPrice, decimal? maxPrice)
    {
        var books = await _bookRepository.GetFilteredBooksAsync(genreId, minPrice, maxPrice);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            books = books.Where(b => 
                b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                b.ISBN.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                (b.Genre != null && b.Genre.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToList();
        }
        var genres = await _bookRepository.GetAllGenresReadOnlyAsync();

        var bookViewModels = books.Select(b => new BookListViewModel
        {
            Id = b.Id,
            ISBN = b.ISBN,
            Title = b.Title,
            Author = b.Author,
            Price = b.Price,
            Quantity = b.Quantity,
            GenreName = b.Genre != null ? b.Genre.Name : "N/A",
            IsLowStock = b.Quantity <= 5 && b.Quantity > 0
        }).ToList();

        return new BookFilterViewModel
        {
            GenreId = genreId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Keyword = keyword,
            Books = bookViewModels
        };
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<BookStatsViewModel> GetBookStatsAsync()
        {
        var books = await _bookRepository.GetAllReadOnlyAsync();
        return new BookStatsViewModel
        {
            TotalItems = books.Count,
            OutOfStockCount = books.Count(b => b.Quantity == 0),
            LowStockCount = books.Count(b => b.Quantity > 0 && b.Quantity <= b.MinStock),
            TotalInventoryValue = books.Sum(b => b.Price * b.Quantity)
        };
    }

    public async Task CreateAsync(BookCreateViewModel model)
    {
        string? finalIsbn = model.ISBN;
        if (string.IsNullOrWhiteSpace(finalIsbn))
        {
            var random1 = new Random().Next(10, 99);
            var random2 = new Random().Next(1000, 9999);
            var random3 = new Random().Next(1, 9);
            
            finalIsbn = $"978-604-{random1}-{random2}-{random3}";
        }
        var newBook = new Book
        {
            ISBN = finalIsbn,
            Title = model.Title, 
            Author = model.Author, 
            Price = model.Price ?? 0, 
            Quantity = model.Quantity ?? 0, 
            MinStock = model.MinStock ?? 0, 
            UpdatedAt = DateTime.Now,
            GenreId = model.GenreId ?? 0
        };

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            newBook.ImagePath = await _fileUploadService.SaveBookImageAsync(model.ImageFile);
        }
        await _bookRepository.AddAsync(newBook);
        await _bookRepository.SaveChangesAsync();

        _logger.LogInformation("Book created. BookId={BookId}", newBook.Id);
        await _context.SaveChangesAsync();
    }


    public async Task<bool> IsISBNUniqueAsync(string isbn, int? excludeId = null)
    {
        var query = _context.Books.IgnoreQueryFilters().Where(b => b.ISBN == isbn);
        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }
        return !await query.AnyAsync();
    }

    public async Task<BookEditViewModel?> GetBookForEditAsync(int id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (book == null) return null;

        return new BookEditViewModel
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Author = book.Author,
            Price = book.Price,
            Quantity = book.Quantity,
            MinStock = book.MinStock,
            GenreId = book.GenreId,
            ImagePath = book.ImagePath,
            RowVersion = book.RowVersion != null ? Convert.ToBase64String(book.RowVersion) : ""            
        };
    }

    public async Task UpdateBookAsync(BookEditViewModel model)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == model.Id);
        if (book == null) throw new Exception("Không tìm thấy sách.");

        var clientRowVersion = Convert.FromBase64String(model.RowVersion ?? "");
            
        if (book.RowVersion != null && book.RowVersion.Length > 0)
        {
            if (!book.RowVersion.SequenceEqual(clientRowVersion))
            {
                throw new DbUpdateConcurrencyException("Dữ liệu đã bị thay đổi bởi người khác.");
            }
        }

        book.Title = model.Title;
        book.ISBN = model.ISBN ?? string.Empty;
        book.Author = model.Author;
        book.Price = model.Price ?? 0;
        book.Quantity = model.Quantity ?? 0;
        book.MinStock = model.MinStock ?? 0;
        book.GenreId = model.GenreId ?? 0;
        book.UpdatedAt = DateTime.Now;

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            string newImagePath = await _fileUploadService.SaveBookImageAsync(model.ImageFile);
            if (!string.IsNullOrEmpty(book.ImagePath))
            {
                string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, book.ImagePath.TrimStart('/'));   
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            book.ImagePath = newImagePath;
        }

        await _context.SaveChangesAsync();
            
        _logger.LogInformation("Đã cập nhật sách và xử lý ảnh thành công. ID={BookId}", model.Id);
    }
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (book == null) return false;

        book.IsDeleted = true;
        book.DeletedAt = DateTime.Now;
        book.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        _logger.LogWarning("Đã xóa mềm sách. ID={BookId}", id);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<BookTrashItemViewModel>> GetTrashItemsAsync()
    {
        return await _context.Books.IgnoreQueryFilters()
            .Where(b => b.IsDeleted).AsNoTracking()
            .Select(e => new BookTrashItemViewModel
            {
                Id = e.Id,
                Title = e.Title,
                ISBN = e.ISBN,
                DeletedAt = e.DeletedAt
            }).ToListAsync();
    }

    public async Task<bool> RestoreAsync(int id)
    {
        var book = await _context.Books.IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted);

        if (book == null) return false;

        book.IsDeleted = false;
        book.DeletedAt = null;
        book.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Đã khôi phục sách. ID={BookId}", id);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<AdjustStockViewModel?> GetBookForAdjustStockAsync(int id)
    {
        var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        if (book == null) return null;

        return new AdjustStockViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Quantity = book.Quantity,
            RowVersion = book.RowVersion != null ? Convert.ToBase64String(book.RowVersion) : ""
        };
    }

    public async Task AdjustStockAsync(AdjustStockViewModel model)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == model.Id);
        if (book == null) throw new Exception("Không tìm thấy sách.");

        var clientRowVersion = Convert.FromBase64String(model.RowVersion ?? "");
        if (book.RowVersion != null && book.RowVersion.Length > 0)
        {
            if (!book.RowVersion.SequenceEqual(clientRowVersion))
            {
                throw new DbUpdateConcurrencyException("Dữ liệu đã bị thay đổi.");
            }
        }

        book.Quantity = model.Quantity;
        book.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Đã điều chỉnh tồn kho. ID={BookId}, NewQty={Quantity}", model.Id, model.Quantity);
    }

    public async Task UpdateImagePathAsync(int id, string imagePath)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            book.ImagePath = imagePath;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Đã cập nhật ảnh thành công cho Sách ID: {BookId}", id);
        }
        else
        {
            _logger.LogWarning("Không thể cập nhật ảnh: Không tìm thấy sách với ID: {BookId}", id);
        }
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        try
        {
            _context.Update(book);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Lỗi xảy ra khi cập nhật sách ID: {Id} trong database.", book.Id);
            return false;
        }
    }
}