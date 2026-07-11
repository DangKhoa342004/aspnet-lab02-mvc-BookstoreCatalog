using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreCatalog.Mvc.Data;
using BookstoreCatalog.Mvc.ViewModels;

namespace BookstoreCatalog.Mvc.Controllers;

public class DataHealthController : Controller
{
    private readonly AppDbContext _context;

    public DataHealthController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DataHealthViewModel();
        try
        {
            model.CanConnectToDatabase = await _context.Database.CanConnectAsync();           
            model.DatabaseProvider = _context.Database.ProviderName ?? "Unknown";

            model.TotalBooks = await _context.Books.AsNoTracking().CountAsync();
            model.TotalGenres = await _context.Genres.AsNoTracking().CountAsync();
            model.TotalSales = await _context.Sales.AsNoTracking().CountAsync();
        }
        catch (Exception)
        {
            model.CanConnectToDatabase = false;
        }

        return View(model);
    }
}