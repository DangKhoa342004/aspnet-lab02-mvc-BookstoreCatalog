using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;
using BookstoreCatalog.Mvc.Repositories;
using BookstoreCatalog.Mvc.Options;
using Microsoft.Extensions.Options;

namespace BookstoreCatalog.Mvc.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly AppSettings _settings;

    public BookService(IBookRepository bookRepository, IOptions<AppSettings> options)
    {
        _bookRepository = bookRepository;
        _settings = options.Value;
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
            Stock = b.Stock,
            GenreName = b.Genre != null ? b.Genre.Name : "N/A",
            IsLowStock = b.Stock < _settings.LowStockThreshold
        }).ToList();
    }

    public async Task<BookDetailViewModel?> GetBookDetailAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null) return null;

        return new BookDetailViewModel
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            Stock = book.Stock,
            GenreName = book.Genre != null ? book.Genre.Name : "N/A",
            IsLowStock = book.Stock < _settings.LowStockThreshold
        };
    }

    public async Task<BookFilterViewModel> GetFilteredBooksViewModelAsync(int? genreId, decimal? minPrice, decimal? maxPrice)
    {
        var books = await _bookRepository.GetFilteredBooksAsync(genreId, minPrice, maxPrice);

        var bookViewModels = books.Select(b => new BookListViewModel
        {
            Id = b.Id,
            ISBN = b.ISBN,
            Title = b.Title,
            Author = b.Author,
            Price = b.Price,
            Stock = b.Stock,
            GenreName = b.Genre != null ? b.Genre.Name : "N/A",
        }).ToList();

    
        return new BookFilterViewModel
        {
            GenreId = genreId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Books = bookViewModels
        };
    }
}