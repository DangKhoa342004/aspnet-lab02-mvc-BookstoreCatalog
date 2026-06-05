namespace BookstoreCatalog.Mvc.ViewModels;

public class BookDetailViewModel
{
    public int Id { get; set; }

    public string ISBN { get; set; } = "";
    
    public string Title { get; set; } = "";

    public string Author { get; set; } = "";

    public string Category { get; set; } = "";

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public int MinStock { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public string PriceText => $"{UnitPrice:N0} VND";

    public decimal InventoryValue => UnitPrice * Quantity;

    public string InventoryValueText => $"{InventoryValue:N0} VND";

    public string LastUpdatedText => LastUpdatedAt.ToString("dd/MM/yyyy HH:mm");

    public string StockStatus
    {
        get
        {
            if (Quantity >= 20)
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

    public string ReorderSuggestion
    {
        get
        {
            if (Quantity <= 0)
            {
                return "Can nhap hang ngay vi san pham da het.";
            }

            if (Quantity <= MinStock)
            {
                return $"Nen nhap them. Ton kho hien tai chi con {Quantity}, muc toi thieu la {MinStock}.";
            }

            return "Ton kho dang on dinh, chua can nhap them.";
        }
    }
}