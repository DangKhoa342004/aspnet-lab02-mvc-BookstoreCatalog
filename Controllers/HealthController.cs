using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;

namespace BookstoreCatalog.Mvc.Controllers;
public class HealthController : Controller
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [Route("health/ready")]
    public async Task<IActionResult> Ready()
    {
        var report = await _healthCheckService.CheckHealthAsync();

        ViewBag.OverallStatus = report.Status == HealthStatus.Healthy ? "Healthy" : "Unhealthy";

        ViewBag.SelfStatus = "Healthy";

        var dbReport = report.Entries.Values.FirstOrDefault(); 
        if (report.Entries.TryGetValue("database", out var dbEntry) && dbEntry.Status == HealthStatus.Healthy)
        {
            ViewBag.DbStatus = "Healthy";
        }
        else
        {
            ViewBag.DbStatus = "Unhealthy";
        }

        return View();
    }
}