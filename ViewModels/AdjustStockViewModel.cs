using System.ComponentModel.DataAnnotations;

namespace BookstoreCatalog.Mvc.ViewModels
{
    public class AdjustStockViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty; // just display

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được phép nhỏ hơn 0.")]
        public int Quantity { get; set; }

        public string? RowVersion { get; set; }
    }
}