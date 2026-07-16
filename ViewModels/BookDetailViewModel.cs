namespace BookstoreCatalog.Mvc.ViewModels;

public class BookDetailViewModel
{
    public int Id { get; set; }
    
    public string Title { get; set; } = "";

    public string ISBN { get; set; } = string.Empty; 

    public string Author { get; set; } = "";

    public string GenreName { get; set; } = "";

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int MinStock{ get; set; }

    public string PriceText => $"{Price:N0} VND";

    public decimal InventoryValue => Price * Quantity;

    public string InventoryValueText => $"{InventoryValue:N0} VND";

    public bool IsLowStock { get; set; }

    public string Suggestion => Quantity <= MinStock 
        ? "Cần nhập thêm sách này để đảm bảo vận hành." 
        : "Số lượng sách đang đủ dùng.";
    
    public string? ImagePath { get; set; }
}