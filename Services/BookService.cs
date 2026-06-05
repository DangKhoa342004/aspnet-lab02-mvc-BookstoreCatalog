using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;

namespace BookstoreCatalog.Mvc.Services;

public class BookService
{
    private readonly List<Book> _books = new()
    {
        new Book
        {
            Id = 1,
            ISBN = "8935280919266",
            Title = "Dac Nhan Tam",
            Author = "Dale Carnegie",
            Category = "Self-Help",
            UnitPrice = 130000,
            Quantity = 18,
            MinStock = 5,
            LastUpdatedAt = new DateTime(2026, 5, 5, 8, 12, 0)
        },
        new Book
        {
            Id = 2,
            ISBN = "8935235226272",
            Title = "Nha Gia Kim",
            Author = "Paulo Coelho",
            Category = "Novel",
            UnitPrice = 95000,
            Quantity = 4,
            MinStock = 5,
            LastUpdatedAt = new DateTime(2026, 5, 5, 8, 16, 0)
        },
        new Book
        {
            Id = 3,
            ISBN = "9786045598351",
            Title = "Thuc sac",
            Author = "Ninh Vien",
            Category = "Romance",
            UnitPrice = 320000,
            Quantity = 0,
            MinStock = 3,
            LastUpdatedAt = new DateTime(2026, 5, 3, 9, 0, 0)
        },
        new Book
        {
            Id = 4,
            ISBN = "9786044809953",
            Title = "Rooms Tuyen tap tranh minh hoa",
            Author = "Senbon Umishima",
            Category = "Chill",
            UnitPrice = 200000,
            Quantity = 9,
            MinStock = 4,
            LastUpdatedAt = new DateTime(2026, 3, 5, 8, 12, 0)
        },
        new Book
        {
            Id = 5,
            ISBN = "9786043828627",
            Title = "Roi hoa se no - Bloom into you",
            Author = "Nakatani Nio",
            Category = "Romance",
            UnitPrice = 1500000,
            Quantity = 2,
            MinStock = 6,
            LastUpdatedAt = new DateTime(2026, 5, 16, 15, 0, 0)
        },
        new Book
        {
            Id = 6,
            ISBN = "8936883231519",
            Title = "Se co cach dung lo",
            Author = "Tue Nghi",
            Category = "Self-Help",
            UnitPrice = 69000,
            Quantity = 0,
            MinStock = 3,
            LastUpdatedAt = new DateTime(2026, 4, 3, 20, 30, 0)
        },
        new Book
        {
            Id = 7,
            ISBN = "9786041198456",
            Title = "Ngay xua co mot chuyen tinh",
            Author = "Nguyen Nhat Anh",
            Category = "Romance",
            UnitPrice = 125000,
            Quantity = 10,
            MinStock = 5,
            LastUpdatedAt = new DateTime(2026, 6, 5, 3, 12, 0)
        },
        new Book
        {
            Id = 8,
            ISBN = "9786043199703",
            Title = "Tuoi tre dang gia bao nhieu",
            Author = "Rossie Nguyen",
            Category = "Self-Help",
            UnitPrice = 90000,
            Quantity = 21,
            MinStock = 10,
            LastUpdatedAt = new DateTime(2026, 5, 5, 15, 12, 0)
        }
    };

    public List<Book> GetAll()
    {
        return _books;
    }

    public Book? GetById(int id)
    {
        return _books.FirstOrDefault(book => book.Id == id);
    }

    public BookStatsViewModel GetStats()
    {
        var totalBooks = _books.Count;

        var totalQuantity = _books.Sum(book => book.Quantity);

        var totalInventoryValue = _books.Sum(book =>
            book.UnitPrice * book.Quantity);

        var outOfStockCount = _books.Count(book =>
            book.Quantity <= 0);

        var needReorderCount = _books.Count(book =>
            book.Quantity > 0 && book.Quantity <= book.MinStock);

        return new BookStatsViewModel
        {
            TotalBooks = totalBooks,
            TotalQuantity = totalQuantity,
            TotalInventoryValue = totalInventoryValue,
            OutOfStockCount = outOfStockCount,
            NeedReorderCount = needReorderCount
        };
    }
}