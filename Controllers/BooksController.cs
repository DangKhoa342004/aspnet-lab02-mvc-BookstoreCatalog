using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.Services;
using BookstoreCatalog.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreCatalog.Mvc.Controllers;

public class BooksController : Controller
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    public IActionResult Index()
    {
        var books = _bookService.GetAll()
            .Select(ToListItemViewModel)
            .ToList();

        return View(books);
    }

    public IActionResult Detail(int id)
    {
        var book = _bookService.GetById(id);

        if (book == null)
        {
            return NotFound($"Khong tim thay sach co id = {id}");
        }

        var viewModel = ToDetailViewModel(book);

        return View(viewModel);
    }

    public IActionResult Stats()
    {
        var stats = _bookService.GetStats();

        return View(stats);
    }

    public IActionResult Welcome()
    {
        return Content("Welcome to ASP.NET Core MVC Lab02");
    }

    public IActionResult ProductJson()
    {
        var books = _bookService.GetAll()
            .Select(book => new
            {
                book.Id,
                book.ISBN,
                book.Title,
                book.Author,
                book.Category,
                book.UnitPrice,
                book.Quantity
            });

        return Json(books);
    }

    public IActionResult GoToList()
    {
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Force404()
    {
        return NotFound("Đây là response 404 demo từ action Force404.");
    }

    public IActionResult CategoryInfo()
    {
        return Content("Danh mục hiện có: Self-Help, Novel, Romance, Chill");
    }

    private static BookListViewModel ToListItemViewModel(Book book)
    {
        return new BookListViewModel
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            UnitPrice = book.UnitPrice,
            Quantity = book.Quantity,
            MinStock = book.MinStock
        };
    }

    private static BookDetailViewModel ToDetailViewModel(Book book)
    {
        return new BookDetailViewModel
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            UnitPrice = book.UnitPrice,
            Quantity = book.Quantity,
            MinStock = book.MinStock,
            LastUpdatedAt = book.LastUpdatedAt
        };
    }
}