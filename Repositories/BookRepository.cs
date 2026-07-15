using Microsoft.EntityFrameworkCore;
using BookstoreCatalog.Mvc.Data;
using BookstoreCatalog.Mvc.Models;
using System.Collections.Generic;

namespace BookstoreCatalog.Mvc.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;
    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Book>> GetAllAsync()
        => _context.Books.Include(b => b.Genre).ToListAsync();

    public Task<List<Book>> GetAllReadOnlyAsync()
        => _context.Books.Include(b => b.Genre).AsNoTracking().ToListAsync();

    public Task<Book?> GetByIdAsync(int id)
        => _context.Books.Include(b => b.Genre).FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddAsync(Book book)
        => await _context.Books.AddAsync(book);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();

    public Task<List<Genre>> GetAllGenresReadOnlyAsync()
        => _context.Genres.AsNoTracking().ToListAsync();

    public async Task<List<Book>> GetFilteredBooksAsync(int? genreId, decimal? minPrice, decimal? maxPrice)
    {
    
        var query = _context.Books.Include(b => b.Genre).AsNoTracking().AsQueryable();
    
        if (genreId.HasValue)
        {
            query = query.Where(b => b.GenreId == genreId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(b => b.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(b => b.Price <= maxPrice.Value);
        }

        return await query.ToListAsync();
    }
}