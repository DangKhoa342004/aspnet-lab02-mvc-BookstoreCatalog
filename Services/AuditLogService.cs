using BookstoreCatalog.Mvc.Data;
using BookstoreCatalog.Mvc.Models;
using Microsoft.AspNetCore.Http;
using System;
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
}