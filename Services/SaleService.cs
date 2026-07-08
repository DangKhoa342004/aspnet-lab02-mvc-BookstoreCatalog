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

	public Task CreateAsync(Sale sale)
		=> _saleRepository.AddAsync(sale);

	public Task CreateSaleAsync(SaleCreateViewModel model)
	{
    	return _saleRepository.CreateSaleAsync(model);
	}
}
