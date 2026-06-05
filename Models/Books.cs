namespace BookstoreCatalog.Mvc.Models;

public class Book
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
}
