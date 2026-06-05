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
                return "Hang con nhieu";
            }

            if (Quantity <= 0)
            {
                return "Het hang";
            }

            if (Quantity <= MinStock)
            {
                return "Can bo sung hang";
            }

            return "Con hang";
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