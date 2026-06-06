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
        return Content("Welcome to Mini Bookstore Catalog API!");
    }

    public IActionResult BookJson()
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
        var categories = _bookService.GetAllCategories();
        return Json(categories);
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

    [HttpGet]
    public IActionResult Search(string? keyword, decimal? minPrice)
    {
        var books = _bookService.Search(keyword, minPrice)
            .Select(ToListItemViewModel)
            .ToList();

        var viewModel = new BookSearchViewModel
        {
            Keyword = keyword ?? "",
            MinPrice = minPrice,
            Books = books
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new BookCreateViewModel
        {
            Quantity = 1,
            MinStock = 1
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _bookService.Create(model);

        TempData["SuccessMessage"] = "Đã thêm sách thành công.";

        return RedirectToAction(nameof(Index));
    }
}