using BookstoreCatalog.Mvc.ViewModels;
using BookstoreCatalog.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookstoreCatalog.Mvc.Controllers;

[Authorize(Policy = "CanViewAuditLog")]
public class AuditLogsController : Controller
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService) 
    {
        _auditLogService = auditLogService;
    }

    public async Task<IActionResult> Index(AuditLogSearchViewModel request)
    {
        var viewModel = await _auditLogService.GetSearchAuditLogsAsync(request);
        
        return View(viewModel);
    } 
}