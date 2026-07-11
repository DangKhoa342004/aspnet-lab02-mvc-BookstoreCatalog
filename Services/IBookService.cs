using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;

public interface IBookService
{
    Task<List<BookListViewModel>> GetBookListAsync();
    Task<BookDetailViewModel?> GetBookDetailAsync(int id);
    Task<BookFilterViewModel> GetFilteredBooksViewModelAsync(int? genreId, decimal? minPrice, decimal? maxPrice);

    Task<Book> GetByIdAsync(int id);
    Task<BookStatsViewModel> GetBookStatsAsync(int id);
    Task CreateAsync(Book book);

    // Kiem tra ISBN doc nhat
    Task<bool> IsISBNUniqueAsync(string isbn, int? excludeBookId = null);

    // Edit book
    Task<BookEditViewModel> GetBookForEditAsync(int id);
    Task UpdateBookAsync(BookEditViewModel model);

    // Delete and Restore book
    Task <bool> SoftDeleteAsync(int id);
    Task<List<BookTrashItemViewModel>> GetTrashItemsAsync();
    Task<bool> RestoreAsync(int id);

    // Feature 1: Adjust stock
    Task<AdjustStockViewModel?> GetBookForAdjustStockAsync(int id);
    Task AdjustStockAsync(AdjustStockViewModel model);
}
