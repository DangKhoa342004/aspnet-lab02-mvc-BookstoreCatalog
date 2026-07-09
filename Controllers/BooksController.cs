using BookstoreCatalog.Mvc.ViewModels;
using BookstoreCatalog.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreCatalog.Mvc.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetBookListAsync();
        return View(books);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var viewModel = await _bookService.GetBookDetailAsync(id);

        if (viewModel == null)
        {
            return NotFound($"Không tìm thấy cuốn sách có id = {id}");
        }

        return View(viewModel);
    }

    public IActionResult Welcome()
    {
        return Content("Welcome to ASP.NET Core MVC Lab04");
    }

    public IActionResult GoToList()
    {
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Force404()
    {
        return NotFound("Đây là response 404 demo từ action Force404.");
    }

    [HttpGet]
    public async Task<IActionResult> Filter(int? genreId, decimal? minPrice, decimal? maxPrice)
    {
        var viewModel = await _bookService.GetFilteredBooksViewModelAsync(genreId, minPrice, maxPrice); 
    
        return View(viewModel);
    }
}