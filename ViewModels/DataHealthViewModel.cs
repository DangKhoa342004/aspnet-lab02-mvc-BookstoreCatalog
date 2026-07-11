using System.Collections.Generic;

namespace BookstoreCatalog.Mvc.ViewModels;

public class HealthCheckItem
{
    public bool CanConnectToDatabase { get; set; }
    public int TotalBooks { get; set; }
    public int TotalGenres { get; set; }
    public int TotalSales { get; set; }
    public string DatabaseProvider { get; set; } = string.Empty;
}