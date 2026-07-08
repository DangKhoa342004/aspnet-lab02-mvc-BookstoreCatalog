using System.ComponentModel.DataAnnotations;

namespace BookstoreCatalog.Mvc.ViewModels;

public class SaleCreateViewModel
{
    [Required(ErrorMessage = "Tên khách hàng không được để trống")]
    [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Mã sách không được để trống")]
    [Range(1, 5, ErrorMessage = "Vui lòng chọn một sản phẩm hợp lệ (1-5)")]
    public int BookId { get; set; }

    [Required(ErrorMessage = "Số lượng không được để trống")]
    [Range(0, 100, ErrorMessage = "Số lượng phải từ 0 đến 100")]
    public int Quantity { get; set; }
}