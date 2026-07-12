using BookstoreCatalog.Mvc.ViewModels;
using BookstoreCatalog.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet]
    public async Task<IActionResult> Search(string? keyword, int? genreId, decimal? minPrice, decimal? maxPrice)
    {
        var viewModel = await _bookService.GetFilteredBooksViewModelAsync(keyword, genreId, minPrice, maxPrice); 
    
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _bookService.CreateAsync(model);
        TempData["SuccessMessage"] = $"Tạo sách '{model.Title}' thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _bookService.GetBookForEditAsync(id);
        
        if (model == null)
        {
            return NotFound($"Không tìm thấy cuốn sách có id = {id} để chỉnh sửa.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _bookService.UpdateBookAsync(model);
            TempData["SuccessMessage"] = $"Cập nhật thông tin sách '{model.Title}' thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError("", "Dữ liệu đã bị thay đổi bởi một người dùng khác. Vui lòng tải lại trang.");
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
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
    public async Task<IActionResult> Trash()
    {
        var trashItems = await _bookService.GetTrashItemsAsync();
        return View(trashItems);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _bookService.RestoreAsync(id);
    
        if (!success)
        {
            return NotFound($"Không tìm thấy sách có id = {id} trong thùng rác hoặc sách chưa bị xóa.");
        }

        TempData["SuccessMessage"] = $"Phục hồi sách có ID = {id} thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _bookService.GetByIdAsync(id);
        
        if (book == null)
        {
            return NotFound($"Không tìm thấy sách có id = {id} để xóa.");
        }

        var success = await _bookService.SoftDeleteAsync(id);
        
        if (!success)
        {
            return BadRequest("Đã xảy ra lỗi, không thể xóa sách này.");
        }

        TempData["SuccessMessage"] = $"Đã chuyển sách '{book.Title}' vào thùng rác thành công!";

        return RedirectToAction(nameof(Trash));
    }

    [HttpGet]
    public async Task<IActionResult> AdjustStock(int id)
    {
        var model = await _bookService.GetBookForAdjustStockAsync(id);

        if (model == null)
        {
            return NotFound($"Không tìm thấy cuốn sách có id = {id} để điều chỉnh tồn kho.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdjustStock(AdjustStockViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _bookService.AdjustStockAsync(model);
            TempData["SuccessMessage"] = $"Cập nhật số lượng sách '{model.Title}' thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError("", "Số lượng tồn kho hoặc dữ liệu sách đã bị thay đổi bởi người khác. Vui lòng tải lại trang.");
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [Route("api/books/{id}")]
    public async Task<IActionResult> ApiErrorDemo(int id)
    {
        var viewModel = await _bookService.GetBookDetailAsync(id);

        var traceId = HttpContext.TraceIdentifier;

        if (viewModel == null)
        {
        ViewBag.BookId = id;
        ViewBag.TraceId = traceId;
        
        return View("ApiError404");
    }

    return View("Detail", viewModel);
}
}