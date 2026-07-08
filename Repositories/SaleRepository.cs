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

	public async Task CreateSaleAsync(SaleCreateViewModel model)
	{
    	await using var transaction = await _context.Database.BeginTransactionAsync();
    	try
    	{
        	var book = await _context.Books.FirstOrDefaultAsync(p => p.Id == model.BookId);
        	if (book == null) throw new Exception("Book not found");
        	if (book.Stock < model.Quantity) throw new Exception("Not enough stock");

        	var sale = new Sale
        	{
				CustomerName = model.CustomerName,
            	CreatedAt = DateTime.Now,
            	TotalAmount = book.Price * model.Quantity
        	};
        	_context.Sales.Add(sale);
            await _context.SaveChangesAsync();

        	var item = new SaleItem
        	{
            	SaleId = sale.Id,
            	BookId = book.Id,
            	Quantity = model.Quantity,
            	UnitPrice = book.Price
        	};
        	_context.SaleItems.Add(item);
        	book.Stock -= model.Quantity;

        	await _context.SaveChangesAsync();
        	await transaction.CommitAsync();
    	}
    	catch
    	{
        	await transaction.RollbackAsync();
        	throw;
    	}
	}
}
