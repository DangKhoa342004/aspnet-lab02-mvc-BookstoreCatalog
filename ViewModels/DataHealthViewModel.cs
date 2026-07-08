using System.Collections.Generic;

namespace BookstoreCatalog.Mvc.ViewModels;

public class HealthCheckItem
{
    public string Check { get; set; } = string.Empty;
    public string Expected { get; set; } = string.Empty;
    public string Actual { get; set; } = string.Empty;
    public string Status { get; set; } = "OK";
    public string Note { get; set; } = string.Empty;
}

public class DataHealthViewModel
{
    public List<HealthCheckItem> Items { get; set; } = new List<HealthCheckItem>();
}