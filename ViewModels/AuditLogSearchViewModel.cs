using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookstoreCatalog.Mvc.Models;

namespace BookstoreCatalog.Mvc.ViewModels;
public class AuditLogSearchViewModel
{
    public string? UserName { get; set; }
    public string? LogAction { get; set; }
    public string? Result { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int AccessDeniedToday { get; set; }
    public int SensitiveActionsToday { get; set; }
    public int RejectedUploadsToday { get; set; }

    public List<SelectListItem> ActionOptions { get; set; } = new List<SelectListItem>();
    public List<AuditLog> Logs { get; set; } = new List<AuditLog>();
}