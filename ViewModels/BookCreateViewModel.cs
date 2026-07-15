using System.ComponentModel.DataAnnotations;

namespace BookstoreCatalog.Mvc.ViewModels;

public class BookCreateViewModel
{
    [Required(ErrorMessage = "Tên sách không được để trống")]
    [StringLength(100, ErrorMessage = "Tên sách không được vượt quá 100 ký tự")]
    public string Title { get; set; } = "";

    [RegularExpression(@"^\d{3}-\d{3}-\d{2}-\d{4}-\d$", 
        ErrorMessage = "ISBN phải đúng định dạng 13 số dạng 3-3-2-4-1 (Ví dụ: 978-604-55-9835-1).")]
    public string? ISBN { get; set; } = "";

    [Required(ErrorMessage = "Tác giả không được để trống")]
    [StringLength(100, ErrorMessage = "Tác giả không được vượt quá 100 ký tự")]
    public string Author { get; set; } = "";

    [Required(ErrorMessage = "Giá bán không được để trống")]
    [Range(1000, 100000000, ErrorMessage = "Giá bán phải từ 1.000 đến 100.000.000")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Số lượng không được để trống")]
    [Range(0, 10000, ErrorMessage = "Số lượng phải từ 0 đến 10.000")]
    public int? Quantity { get; set; }

    [Required(ErrorMessage = "Số lượng tối thiểu không được để trống")]
    [Range(0, 10000, ErrorMessage = "Mức tồn tối thiểu phải từ 0 đến 100.000")]
    public int? MinStock { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn Mã Thể loại.")]
    [Range(1, 3, ErrorMessage = "1:Romance, 2:Chill, 3:Seft-help")]
    public int? GenreId { get; set; }
}