using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Gatepass : BaseEntity
    {
        public Guid GatePassRequestId { get; set; }
        public GatepassRequest GatepassRequest { get; set; } = new GatepassRequest();
        public string UniqueCode { get; set; } = string.Empty;
        public string QRCodeImage { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }

        public string AccessPoints { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public bool IsRevoked { get; set; }
        public Guid? RevokedById { get; set; }
        public User? RevokedBy { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? RevokedReason { get; set; }
        public int UsageCount { get; set; }

        public List<CheckInOut> CheckInOuts { get; set; } = new List<CheckInOut>();
    }
}
