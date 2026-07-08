using BookstoreCatalog.Mvc.ViewModels;

public interface IBookService
{
    Task<List<BookListViewModel>> GetBookListAsync();
    Task<BookDetailViewModel?> GetBookDetailAsync(int id);
    Task<BookFilterViewModel> GetFilteredBooksViewModelAsync(int? genreId, decimal? minPrice, decimal? maxPrice);
}
