using BookstoreCatalog.Mvc.Data;
using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookstoreCatalog.Mvc.Repositories;

public class SaleRepository : ISaleRepository
{
	private readonly AppDbContext _context;

	public SaleRepository(AppDbContext context)
	{
		_context = context;
	}

	public Task<List<Sale>> GetAllAsync()
		=> _context.Sales
				   .Include(s => s.SaleItems)
					   .ThenInclude(si => si.Book)
				   .OrderByDescending(s => s.CreatedAt)
        			.ToListAsync();

	public Task<List<Sale>> GetAllReadOnlyAsync()
        => _context.Sales
                   .Include(s => s.SaleItems)
                       .ThenInclude(si => si.Book)
                   .AsNoTracking()
                   .ToListAsync();

	public Task<Sale?> GetByIdAsync(int id)
		=> _context.Sales
				   .Include(s => s.SaleItems)
					   .ThenInclude(si => si.Book)
				   .FirstOrDefaultAsync(s => s.Id == id);

	public async Task AddAsync(Sale sale)
		=> await _context.Sales.AddAsync(sale);

	public Task SaveChangesAsync()
		=> _context.SaveChangesAsync();
}
