using System.ComponentModel.DataAnnotations;

namespace BookstoreCatalog.Mvc.ViewModels;

public class BookCreateViewModel
{
    [Required(ErrorMessage = "Tên sách không được để trống")]
    [StringLength(100, ErrorMessage = "Tên sách không được vượt quá 100 ký tự")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Tác giả không được để trống")]
    [StringLength(100, ErrorMessage = "Tác giả không được vượt quá 100 ký tự")]
    public string Author { get; set; } = "";

    [Range(1000, 100000000, ErrorMessage = "Giá bán phải từ 1.000 đến 100.000.000")]
    public decimal Price { get; set; }

    [Range(0, 10000, ErrorMessage = "Số lượng phải từ 0 đến 10.000")]
    public int Quantity { get; set; }

    [Range(0, 10000, ErrorMessage = "Mức tồn tối thiểu phải từ 0 đến 100.000")]
    public int MinStock { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn thể loại.")]
    public int GenreId { get; set; }
}