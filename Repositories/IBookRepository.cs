using BookstoreCatalog.Mvc.Models;
using System.Collections.Generic;

namespace BookstoreCatalog.Mvc.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<List<Book>> GetAllReadOnlyAsync();
    Task<Book?> GetByIdAsync(int id);
    Task AddAsync(Book book);
    Task SaveChangesAsync();
    Task<List<Book>> GetFilteredBooksAsync(int? genreId, decimal? minPrice, decimal? maxPrice);
    Task<List<Genre>> GetAllGenresReadOnlyAsync();
}