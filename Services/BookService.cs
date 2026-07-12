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

    public BookService(IBookRepository bookRepository, IOptions<AppSettings> options, AppDbContext context, ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _settings = options.Value;
        _context = context;
        _logger = logger;
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
            IsLowStock = b.Quantity < _settings.LowStockThreshold && b.Quantity > 0
        }).ToList();
    }

    public async Task<BookDetailViewModel?> GetBookDetailAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            _logger.LogError("Invalid request: BookId={BookId}", id);
            var log = new AuditLogs
            {
                Level = "Error",
                Message = $"Invalid request: ProductId={id} TraceId={Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                Time = DateTime.Now
            };
            _context.AuditLogs.Add(log);
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
        var books = await _bookRepository.GetAllReadOnlyAsync();
        var random1 = new Random().Next(10, 99);
        var random2 = new Random().Next(1000, 9999);
        var random3 = new Random().Next(1, 9);

        var newBook = new Book
        {
            ISBN = $"978-604-{random1}-{random2}-{random3}",
            Title = model.Title, 
            Author = model.Author, 
            Price = model.Price, 
            Quantity = model.Quantity, 
            MinStock = model.MinStock, 
            UpdatedAt = DateTime.Now,
            GenreId = model.GenreId
        };

        await _bookRepository.AddAsync(newBook);
        await _bookRepository.SaveChangesAsync();

        _logger.LogInformation("Book created. BookId={BookId}", newBook.Id);
        var log = new AuditLogs
            {
                Level = "Information",
                Message = $"Product created. ProductId={newBook.Id}",
                Time = DateTime.Now
            };
        _context.AuditLogs.Add(log);
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
                
            RowVersion = book.RowVersion != null ? Convert.ToBase64String(book.RowVersion) : ""            
        };
    }

    public async Task UpdateBookAsync(BookEditViewModel model)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == model.Id);
        if (book == null) throw new Exception("Không tìm thấy sách.");

        // Manual Check
        var clientRowVersion = Convert.FromBase64String(model.RowVersion ?? "");
            
        if (book.RowVersion != null && book.RowVersion.Length > 0)
        {
            if (!book.RowVersion.SequenceEqual(clientRowVersion))
            {
                throw new DbUpdateConcurrencyException("Dữ liệu đã bị thay đổi bởi người khác.");
            }
        }

        book.Title = model.Title;
        book.ISBN = model.ISBN;
        book.Author = model.Author;
        book.Price = model.Price;
        book.Quantity = model.Quantity;
        book.MinStock = model.MinStock;
        book.GenreId = model.GenreId;
        book.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
            
        _logger.LogInformation("Đã cập nhật sách. ID={BookId}", model.Id);
        var log = new AuditLogs
        {
            Level = "Information",
            Message = $"Product updated. ProductId={model.Id}",
            Time = DateTime.Now
        };
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
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
        var log = new AuditLogs
        {
            Level = "Warning",
            Message = $"Product soft deleted. ProductId={id}",
            Time = DateTime.Now
        };
        _context.AuditLogs.Add(log);
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
        var log = new AuditLogs
        {
            Level = "Information",
            Message = $"Product restored. ProductId={id}",
            Time = DateTime.Now
        };
        _context.AuditLogs.Add(log);
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
}