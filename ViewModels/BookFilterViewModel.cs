using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookstoreCatalog.Mvc.ViewModels;

public class BookFilterViewModel
{
    [Range(1, 3, ErrorMessage = "Thể loại hiện có: 1 - Romance, 2 - Chill, 3 - Self-Help")]
    public int? GenreId { get; set; }

    [Range(0, 100000000, ErrorMessage = "Giá tối thiểu không được nhỏ hơn 0.")]
    public decimal? MinPrice { get; set;}

    [Range(0, 100000000, ErrorMessage = "Giá tối đa không được nhỏ hơn 0.")]
    public decimal? MaxPrice { get; set;}

    public List<BookListViewModel> Books { get; set; } = new();

    public List<SelectListItem> GenreOptions { get; set; } = new();
}