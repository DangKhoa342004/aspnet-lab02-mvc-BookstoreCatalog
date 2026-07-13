using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BookstoreCatalog.Mvc.Controllers;
public class HealthController : Controller
{
    private readonly HealthCheckService _healthCheckService;
    private readonly IWebHostEnvironment _env;

    public HealthController(HealthCheckService healthCheckService, IWebHostEnvironment env)
    {
        _healthCheckService = healthCheckService;
        _env = env;
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

    [Route("health/live")]
    public async Task<IActionResult> Live()
    {
        var report = await _healthCheckService.CheckHealthAsync(check => check.Tags.Contains("live"));
        ViewBag.LiveStatus = report.Status.ToString(); 
        
        var startTime = Process.GetCurrentProcess().StartTime;
        var uptime = DateTime.Now - startTime;
            
        ViewBag.Uptime = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";

        long allocatedMemory = Process.GetCurrentProcess().PrivateMemorySize64;
        ViewBag.MemoryUsage = $"{allocatedMemory / (1024 * 1024)} MB";

        ViewBag.Environment = _env.EnvironmentName;

        return View();
    }
}