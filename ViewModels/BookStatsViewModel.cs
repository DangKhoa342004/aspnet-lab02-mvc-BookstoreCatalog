namespace BookstoreCatalog.Mvc.ViewModels;

public class BookStatsViewModel
{
    public int TotalItems { get; set; }
    public int OutOfStockCount { get; set; }
    public int LowStockCount { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public string TotalInventoryValueText => $"{TotalInventoryValue:N0} VND";
}