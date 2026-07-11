namespace BookstoreCatalog.Mvc.ViewModels;

public class BookListViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string ISBN { get; set; } = string.Empty;

    public string Author { get; set; } = "";

    public string GenreName { get; set; } = "";

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string PriceText => $"{Price:N0} VND";

    public decimal InventoryValue => Price * Stock;

    public string InventoryValueText => $"{InventoryValue:N0} VND";

    public bool IsLowStock { get; set; }

    public string Status 
    {
        get 
        {
            if (Quantity == 0) return "Hết hàng";
            if (IsLowStock) return "Sắp hết";
            return "Sẵn sàng";
        }
    }
}