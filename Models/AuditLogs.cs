using System;

namespace BookstoreCatalog.Mvc.Models
{
    public class AuditLogs
    {
        public int Id { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Level { get; set; } = "Information";        
        public string Message { get; set; } = "";
    }
}