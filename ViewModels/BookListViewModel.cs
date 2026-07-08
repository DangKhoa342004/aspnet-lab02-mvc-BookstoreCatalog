namespace BookstoreCatalog.Mvc.ViewModels;

public class BookListViewModel
{
    public int Id { get; set; }

    public string ISBN { get; set; } = "";

    public string Title { get; set; } = "";

    public string Author { get; set; } = "";

    public string Category { get; set; } = "";

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public int MinStock { get; set; }

    public string PriceText => $"{UnitPrice:N0} VND";

    public decimal InventoryValue => UnitPrice * Quantity;

    public string InventoryValueText => $"{InventoryValue:N0} VND";

    public string StockStatus
    {
        get
        {
            if (Quantity >= 15)
            {
                return "Hàng còn nhiều";
            }

            if (Quantity <= 0)
            {
                return "Hết hàng";
            }

            if (Quantity <= MinStock)
            {
                return "Cần bổ sung hàng";
            }

            return "Còn hàng";
        }
    }

    public string StockStatusClass
    {
        get
        {
            if (Quantity <= 0)
            {
                return "badge badge-danger";
            }

            if (Quantity <= MinStock)
            {
                return "badge badge-warning";
            }

            return "badge badge-success";
        }
    }
}