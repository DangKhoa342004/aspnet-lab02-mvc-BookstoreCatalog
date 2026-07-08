using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;
using System.Collections.Generic;

namespace BookstoreCatalog.Mvc.Services;

public interface ISaleService
{
	Task<List<Sale>> GetAllAsync();
	Task<Sale?> GetByIdAsync(int id);
	Task CreateAsync(Sale sale);
	Task CreateSaleAsync(SaleCreateViewModel model);

}
