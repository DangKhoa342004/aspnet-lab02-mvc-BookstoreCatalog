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
            Title = "Đắc Nhân Tâm",
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
            Title = "Nhà Giả Kim",
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
            Title = "Thực sắc",
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
            Title = "Rooms Tuyển tập tranh minh họa",
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
            Title = "Rồi hoa sẽ nở - Bloom into you",
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
            Title = "Sẽ có cách đừng lo",
            Author = "Tuệ Nghi",
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
            Title = "Ngày xưa có một chuyện tình",
            Author = "Nguyễn Nhật Ánh",
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
            Title = "Tuổi trẻ đáng giá bao nhiêu",
            Author = "Rossie Nguyễn",
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

    public string GenerateNewIsbn()
    {
        var chars = new char[13];
        for (int i = 0; i < 13; i++)
        {
            chars[i] = (char)('0' + Random.Shared.Next(0, 10));
        }

        return new string(chars);
    }

    public List<Book> Search(string? keyword, decimal? minPrice)
    {
        var query = _books.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(book =>
                book.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                book.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                book.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                book.ISBN.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(book => book.UnitPrice >= minPrice.Value);
        }

        return query.ToList();
    }

    public Book Create(BookCreateViewModel model)
    {
        var newId = _books.Count == 0
            ? 1
            : _books.Max(book => book.Id) + 1;

        var newISBN = string.Empty;
        if (string.IsNullOrEmpty(newISBN))
        {
            newISBN = GenerateNewIsbn();
        }

        var book = new Book
        {
            Id = newId,
            ISBN = newISBN,
            Title = model.Title,
            Author = model.Author,
            Category = model.Category,
            UnitPrice = model.UnitPrice,
            Quantity = model.Quantity,
            MinStock = model.MinStock,
            LastUpdatedAt = DateTime.Now
        };

        _books.Add(book);

        return book;
    }
}