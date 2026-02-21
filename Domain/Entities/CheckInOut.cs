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
        public Guid CheckInAccessPointId { get; set; }
        public AccessPoint CheckInAccessPoint { get; set; } = new AccessPoint();
        public Guid CheckInPersonnelId { get; set; }
        public User CheckInPersonnel { get; set; } = new User();
        public DateTime? CheckOutTime { get; set; }
        public Guid? CheckOutAccessPointId { get; set; }
        public AccessPoint? CheckOutAccessPoint { get; set; }
        public Guid? CheckOutPersonnelId { get; set; }
        public User? CheckOutPersonnel { get; set; }
        public DateTime ActualDuration { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool IsOverstay { get; set; }
    }
}
