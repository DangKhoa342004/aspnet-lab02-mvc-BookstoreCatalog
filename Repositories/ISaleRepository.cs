using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;
using System.Collections.Generic;

namespace BookstoreCatalog.Mvc.Repositories;

public interface ISaleRepository
{
	Task<List<Sale>> GetAllAsync();
	Task<List<Sale>> GetAllReadOnlyAsync();
	Task<Sale?> GetByIdAsync(int id);
	Task AddAsync(Sale sale);
	Task SaveChangesAsync();
	Task CreateSaleAsync(SaleCreateViewModel model);
}
