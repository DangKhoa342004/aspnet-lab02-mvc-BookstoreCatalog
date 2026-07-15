using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreCatalog.Mvc.Data;
using BookstoreCatalog.Mvc.Models;
using System.Reflection.PortableExecutable;

namespace BookstoreCatalog.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var totalBooks = await _context.Books.IgnoreQueryFilters().AsNoTracking().CountAsync();
        var totalSales = await _context.Sales.IgnoreQueryFilters().AsNoTracking().CountAsync();
        

        ViewBag.Books = totalBooks;
        ViewBag.Sales = totalSales;

        if (User.IsInRole("Admin") || User.IsInRole("Staff"))
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.AuditLogCount = await _context.AuditLogs.AsNoTracking()
                    .CountAsync(l => l.Action == "Delete" || l.Action == "AdjustStock" 
                        || l.Action == "Restore" || l.Action == "Edit" || l.Action == "UploadImage");
            }
            ViewBag.LowStockCount = await _context.Books.AsNoTracking()
                    .CountAsync(b => !b.IsDeleted && b.Quantity < 5);
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
