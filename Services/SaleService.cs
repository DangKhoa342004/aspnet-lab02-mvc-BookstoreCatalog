using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;
using BookstoreCatalog.Mvc.Repositories;
using BookstoreCatalog.Mvc.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookstoreCatalog.Mvc.Services;

public class SaleService : ISaleService
{
	private readonly ISaleRepository _saleRepository;
	private readonly AppDbContext _context;

	public SaleService(ISaleRepository saleRepository,AppDbContext context)
	{
		_saleRepository = saleRepository;
		_context = context;
	}

	public Task<List<Sale>> GetAllAsync()
		=> _saleRepository.GetAllAsync();

	public Task<Sale?> GetByIdAsync(int id)
		=> _saleRepository.GetByIdAsync(id);

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
