using BookstoreCatalog.Mvc.Services;
using BookstoreCatalog.Mvc.Repositories;
using BookstoreCatalog.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.Design;

namespace BookstoreCatalog.Mvc.Controllers;

[Authorize(Policy = "CanViewSale")]
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
   [Authorize(Policy = "CanManageSale")]
    public async Task<IActionResult> Create()
    {
        var books = await _bookRepository.GetAllReadOnlyAsync();
        ViewBag.BookList = new SelectList(books, "Id", "Title");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "CanManageSale")]
    public async Task<IActionResult> Create(SaleCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var books = await _bookRepository.GetAllReadOnlyAsync();
            ViewBag.BookList = new SelectList(books, "Id", "Name");
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