namespace BookstoreCatalog.Mvc.ViewModels;

public class BookSearchViewModel
{
    public string Keyword { get; set; } = "";

    public decimal? MinPrice { get; set; }

    public List<BookListViewModel> Books { get; set; } = new();
}