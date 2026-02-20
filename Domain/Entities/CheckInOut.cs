using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class CheckInOut : BaseEntity
    {
        public Guid GatePassId { get; set; }
        public Gatepass Gatepass { get; set; } = new Gatepass();
        public DateTime CheckInTime { get; set; }
        public string CheckInGate { get; set; } = string.Empty;
        public User CheckInPersonnelId { get; set; } = new User();
        public DateTime? CheckOutTime { get; set; }
        public string? CheckOutGate { get; set; }
        public User? CheckOutPersonnelId { get; set; } = new User();
        public DateTime ActualDuration { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool IsOverstay { get; set; }
    }
}
