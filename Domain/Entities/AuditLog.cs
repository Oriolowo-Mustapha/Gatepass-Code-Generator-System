using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Guid? UserID { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public Guid? EntityID { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IPAddress { get; set; }
        public string? MachineName { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
