using BookstoreCatalog.Mvc.ViewModels;
using BookstoreCatalog.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.Design;

namespace BookstoreCatalog.Mvc.Controllers;

[Authorize(Policy = "CanViewBook")]
public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IAuditLogService _auditLogService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public BooksController(IBookService bookService, IAuditLogService auditLogService, IFileUploadService fileUploadService, IWebHostEnvironment webHostEnvironment)
    {
        _bookService = bookService;
        _auditLogService = auditLogService;
        _fileUploadService = fileUploadService;
        _webHostEnvironment = webHostEnvironment;
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

    public async Task<IActionResult> Stats()
    {
        var books = await _bookService.GetBookStatsAsync();
        return View(books);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? keyword, int? genreId, decimal? minPrice, decimal? maxPrice)
    {
        var viewModel = await _bookService.GetFilteredBooksViewModelAsync(keyword, genreId, minPrice, maxPrice); 
    
        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Policy = "CanManageBook")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "CanManageBook")]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.ISBN))
        {
            bool isUnique = await _bookService.IsISBNUniqueAsync(model.ISBN);
            if (!isUnique)
            {
                ModelState.AddModelError("ISBN", "Mã ISBN này đã tồn tại trong hệ thống. Vui lòng để trống hoặc nhập mã khác.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _bookService.CreateAsync(model);
        await _auditLogService.LogAsync("Create", "Book", model.ISBN, "Success", $"Tạo sách: {model.Title}");
        TempData["SuccessMessage"] = $"Tạo sách '{model.Title}' thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "CanManageBook")]
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
    [Authorize(Policy = "CanManageBook")]
    public async Task<IActionResult> Edit(BookEditViewModel model)
    {
        bool isUnique = await _bookService.IsISBNUniqueAsync(model.ISBN ?? "", model.Id);
        if (!isUnique)
        {
            ModelState.AddModelError("ISBN", "Mã ISBN này đã được sử dụng bởi một cuốn sách khác.");
        }

        if (ModelState.IsValid)
        {

            try
            {
                await _bookService.UpdateBookAsync(model);
                await _auditLogService.LogAsync("Edit", "Book", model.Id.ToString(), "Success", "Cập nhật thông tin sách.");
                TempData["SuccessMessage"] = $"Cập nhật thông tin sách '{model.Title}' thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Dữ liệu đã bị thay đổi bởi một người dùng khác. Vui lòng tải lại trang.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        return View(model);
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
    [Authorize(Policy = "CanManageBook")]
    public async Task<IActionResult> Trash()
    {
        var trashItems = await _bookService.GetTrashItemsAsync();
        return View(trashItems);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "CanManageBook")]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _bookService.RestoreAsync(id);
    
        if (!success)
        {
            return NotFound($"Không tìm thấy sách có id = {id} trong thùng rác hoặc sách chưa bị xóa.");
        }
        await _auditLogService.LogAsync("Restore", "Book", id.ToString(), "Success", "Phục hồi sách từ thùng rác.");
        TempData["SuccessMessage"] = $"Phục hồi sách có ID = {id} thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "CanManageBook")]
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
        await _auditLogService.LogAsync("SoftDelete", "Book", id.ToString(), "Success", "Đã xóa mềm sách (đưa vào thùng rác).");
        TempData["SuccessMessage"] = $"Đã chuyển sách '{book.Title}' vào thùng rác thành công!";
        return RedirectToAction(nameof(Trash));
    }

    [HttpGet]
    [Authorize(Policy = "CanAdjustStock")]
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
    [Authorize(Policy = "CanAdjustStock")]
    public async Task<IActionResult> AdjustStock(AdjustStockViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _bookService.AdjustStockAsync(model);
            await _auditLogService.LogAsync("AdjustStock", "Book", model.Id.ToString(), "Success", $"Cập nhật số lượng sách.");
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
            await _auditLogService.LogAsync("AdjustStock", "Book", model.Id.ToString(), "Failed", "Dữ liệu đã bị người khác sửa.");
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

    [Route("api-json/books/{id}")]
    public async Task<IActionResult> ApiErrorJsonDemo(int id)
    {
        var viewModel = await _bookService.GetBookDetailAsync(id);
        var traceId = HttpContext.TraceIdentifier;

        if (viewModel == null)
        {
            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Type = "https://example.com/problems/book-not-found",
                Title = "Book not found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The book with id {id} was not found.",
                Instance = HttpContext.Request.Path
            };

            problemDetails.Extensions.Add("traceId", traceId);
            problemDetails.Extensions.Add("errorCode", "BOOK_NOT_FOUND");

            Response.ContentType = "application/problem+json";

            return StatusCode(StatusCodes.Status404NotFound, problemDetails);
        }

        return Json(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "CanUploadBookImage")]
    public async Task<IActionResult> UploadImage(int id, IFormFile imageFile)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book == null) return NotFound();

        if (imageFile == null || imageFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Vui lòng chọn một file để tải lên.";
            return RedirectToAction("Edit", new { id });
        }

        var oldImagePath = book.ImagePath;
        string newImagePath;

        try
        {
            newImagePath = await _fileUploadService.SaveBookImageAsync(imageFile); //
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Edit", new { id });
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Đã xảy ra lỗi hệ thống trong quá trình tải ảnh lên.";
            return RedirectToAction("Edit", new { id });
        }

        book.ImagePath = newImagePath;
        var updateResult = await _bookService.UpdateAsync(book);

        if (updateResult)
        {
            if (!string.IsNullOrEmpty(oldImagePath))
            {
                var absoluteOldPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath.TrimStart('/'));
                if (System.IO.File.Exists(absoluteOldPath))
                {
                    System.IO.File.Delete(absoluteOldPath);
                }
            }

            await _auditLogService.LogAsync("UploadImage", "Book", id.ToString(), "Success", "Thay đổi ảnh sản phẩm thành công.");
            TempData["SuccessMessage"] = "Thay ảnh sản phẩm thành công và an toàn!";
        }
        else
        {
            var absoluteNewPath = Path.Combine(_webHostEnvironment.WebRootPath, newImagePath.TrimStart('/'));
            if (System.IO.File.Exists(absoluteNewPath))
            {
                System.IO.File.Delete(absoluteNewPath);
            }
            
            await _auditLogService.LogAsync("UploadImage", "Book", id.ToString(), "Failed", "Lỗi lưu DB khi cập nhật ảnh mới.");
            TempData["ErrorMessage"] = "Lỗi hệ thống khi cập nhật cơ sở dữ liệu. Ảnh cũ vẫn được giữ nguyên.";
        }

        return RedirectToAction("Edit", new { id });
    }
}