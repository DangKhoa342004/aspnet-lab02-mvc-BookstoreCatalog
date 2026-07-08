using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookstoreCatalog.Mvc.ViewModels;

public class BookFilterViewModel
{
    public int? GenreId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public List<BookListViewModel> Books { get; set; } = new();

    public List<SelectListItem> GenreOptions { get; set; } = new();
}