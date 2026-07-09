namespace BookstoreCatalog.Mvc.Models;

public class Book
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = "";

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int GenreId { get; set; }

    public Genre? Genre { get; set; }

    public ICollection<SaleItem>? SaleItems { get; set; } = new List<SaleItem>();

    public string ISBN { get; set; } = string.Empty; 
}
