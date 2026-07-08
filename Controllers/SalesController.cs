using Microsoft.AspNetCore.Mvc;
using BookstoreCatalog.Mvc.Services;
using BookstoreCatalog.Mvc.ViewModels;
using System;
using System.Threading.Tasks;

namespace BookstoreCatalog.Mvc.Controllers;

public class SalesController : Controller
{
    private readonly ISaleService _saleService;
    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sales = await _saleService.GetAllAsync();
        return View(sales);
    }

   [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _saleService.CreateSaleAsync(model);

            TempData["SuccessMessage"] = "Tạo đơn hàng bán sách thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            string realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
    
            ModelState.AddModelError("", "Chi tiết lỗi: " + realError);
    
            return View(model);
        }
    }
}