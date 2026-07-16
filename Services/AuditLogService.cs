using BookstoreCatalog.Mvc.Data;
using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreCatalog.Mvc.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(string action, string entityName, string? entityId, string result, string? note = null)
    {
        var log = new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            UserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Anonymous",
            IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            Result = result,
            Note = note,
            Time = DateTime.Now
        };
        
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<AuditLogSearchViewModel> GetSearchAuditLogsAsync(AuditLogSearchViewModel request)
    {
        request ??= new AuditLogSearchViewModel();

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var query = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.LogAction))
            query = query.Where(l => l.Action == request.LogAction);
            
        if (!string.IsNullOrEmpty(request.UserName))
            query = query.Where(l => l.UserName != null && l.UserName.Contains(request.UserName));

        if (!string.IsNullOrEmpty(request.Result))
            query = query.Where(l => l.Result == request.Result);

        if (request.StartDate.HasValue)
        {
            var start = request.StartDate.Value.Date;
            query = query.Where(l => l.Time >= start);
        }

        if (request.EndDate.HasValue)
        {
            var end = request.EndDate.Value.Date.AddDays(1);
            query = query.Where(l => l.Time < end);
        }

        request.Logs = await query.OrderByDescending(l => l.Time).ToListAsync();

        var availableActions = await _context.AuditLogs
            .Select(l => l.Action)
            .Distinct()
            .OrderBy(action => action)
            .ToListAsync();

        request.ActionOptions = availableActions
            .Select(action => new SelectListItem
            {
                Value = action,
                Text = action,
                Selected = action == request.LogAction
            })
            .ToList();

        request.AccessDeniedToday = await _context.AuditLogs.AsNoTracking()
            .CountAsync(l => l.Action == "AccessDenied" && l.Time >= today && l.Time < tomorrow);

        request.SensitiveActionsToday = await _context.AuditLogs.AsNoTracking()
            .CountAsync(l => (l.Action == "AdjustStock" || l.Action == "Edit" || l.Action == "Restore" || l.Action == "SoftDelete")
                && l.Time >= today && l.Time < tomorrow);

        request.RejectedUploadsToday = await _context.AuditLogs.AsNoTracking()
            .CountAsync(l => l.Result == "Failed" && l.Action == "UploadImage" && l.Time >= today && l.Time < tomorrow);

        return request; 
    }
}