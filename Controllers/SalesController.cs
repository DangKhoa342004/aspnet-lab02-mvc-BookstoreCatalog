using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookstoreCatalog.Mvc.Services;
using BookstoreCatalog.Mvc.Repositories;
using BookstoreCatalog.Mvc.ViewModels;

namespace BookstoreCatalog.Mvc.Controllers;

public class SalesController : Controller
{
    private readonly ISaleService _saleService;
    private readonly IBookRepository _bookRepository;
    public SalesController(ISaleService saleService, IBookRepository bookRepository)
    {
        _saleService = saleService;
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sales = await _saleService.GetAllAsync();
        return View(sales);
    }

   [HttpGet]
    public async Task<IActionResult> Create()
    {
        var books = await _bookRepository.GetAllReadOnlyAsync();
        ViewBag.BookList = new SelectList(books, "Id", "Title");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var equipments = await _equipmentRepository.GetAllReadOnlyAsync();
            ViewBag.EquipmentList = new SelectList(equipments, "Id", "Name");
            return View(model);
        }

        try
        {
            await _saleService.CreateSaleAsync(model);

            TempData["SuccessMessage"] = "Tạo đơn hàng bán sách thành công!";
            return RedirectToAction("Index", "Books");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);

            var books = await _bookRepository.GetAllReadOnlyAsync();
            ViewBag.BookList = new SelectList(books, "Id", "Title");

            return View(model);
        }
    }
}