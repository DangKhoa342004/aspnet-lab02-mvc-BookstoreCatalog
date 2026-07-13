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
        var activeBooks = await _context.Books.AsNoTracking().CountAsync(b => !b.IsDeleted);
        var deletedBooks = await _context.Books.IgnoreQueryFilters().AsNoTracking().CountAsync(b => b.IsDeleted);
        var logsCountToday = await _context.AuditLogs.CountAsync(l => l.Time >= today);

        ViewBag.Total = totalBooks;
        ViewBag.Active = activeBooks;
        ViewBag.Deleted = deletedBooks;
        ViewBag.Logs = logsCountToday;

        ViewBag.LowStockCount = await _context.Books.AsNoTracking()
            .CountAsync(b => !b.IsDeleted && b.Quantity < 5);
        
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
