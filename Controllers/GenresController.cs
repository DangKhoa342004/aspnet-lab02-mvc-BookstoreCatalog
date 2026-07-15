using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreCatalog.Mvc.Data;

namespace BookstoreCatalog.Mvc.Controllers;

public class GenresController : Controller
{
    private readonly AppDbContext _context;

    public GenresController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var genres = await _context.Genres.Include(g => g.Books).AsNoTracking().ToListAsync();

        return View(genres);
    }
}