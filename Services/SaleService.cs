using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;
using BookstoreCatalog.Mvc.Repositories;
using BookstoreCatalog.Mvc.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookstoreCatalog.Mvc.Services;

public class SaleService : ISaleService
{
	private readonly ISaleRepository _saleRepository;
	private readonly AppDbContext _context;
	private readonly ILogger<SaleService> _logger;

	public SaleService(ISaleRepository saleRepository,AppDbContext context, ILogger<SaleService> logger)
	{
		_saleRepository = saleRepository;
		_context = context;
		_logger = logger;
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
        	if (book.Quantity < model.Quantity) throw new Exception("Not enough stock");

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
        	book.Quantity -= model.Quantity;

			var auditLog = new AuditLog
			{
				Action = "Purchase",
				EntityName = "Book",
				EntityId = book.Id.ToString(),
				UserName = model.CustomerName,
				Result = "Success",
				Time = DateTime.UtcNow,
				Note = $"{model.CustomerName} đã mua sách ID {book.Id} với số lượng {model.Quantity}. Tổng tiền: {book.Price * model.Quantity}"
			};
			_context.AuditLogs.Add(auditLog);

        	await _context.SaveChangesAsync();
        	await transaction.CommitAsync();

			_logger.LogInformation("{CustomerName} đã mua sách {BookId} với số lượng {Quantity}", 
                model.CustomerName, model.BookId, model.Quantity);
    	}
    	catch (Exception ex)
    	{
        	await transaction.RollbackAsync();
			_logger.LogError(ex, "Mua hàng thất bại cho khách hàng {CustomerName}, Sách ID: {BookId}", 
                model.CustomerName, model.BookId);
        	throw;
    	}
	}
}