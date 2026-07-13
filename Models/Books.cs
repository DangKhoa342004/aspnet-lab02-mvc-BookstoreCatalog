using System.ComponentModel.DataAnnotations;

namespace BookstoreCatalog.Mvc.Models;

public class Book
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int MinStock { get; set; }

    // relationships
    public int GenreId { get; set; }
    public Genre? Genre { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<SaleItem>? SaleItems { get; set; } = new List<SaleItem>();

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
