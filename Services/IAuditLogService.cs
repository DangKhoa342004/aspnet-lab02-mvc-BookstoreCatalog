using System.Threading.Tasks;
using BookstoreCatalog.Mvc.ViewModels;

namespace BookstoreCatalog.Mvc.Services;

public interface IAuditLogService
{
    Task LogAsync(string action, string entityName, string? entityId, string result, string? note = null);
    Task<AuditLogSearchViewModel> GetSearchAuditLogsAsync(AuditLogSearchViewModel request);
}