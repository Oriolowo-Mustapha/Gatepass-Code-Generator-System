using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class GatepassRequest : BaseEntity
    {
        public DateTime RequestDate { get; set;  } = DateTime.UtcNow;

        public Guid VisitorsId { get; set; }
        public Visitor Visitor { get; set; } = new Visitor();

        public string VisitPurpose { get; set;  } = string.Empty;

        public Guid? DestinationDepartmentId { get; set; }
        public Department? DestinationDepartment { get; set; } 

        public  Guid HostUserId { get; set; }
        public User HostUser { get; set; } = new User();

        public DateTime RequestedDate { get; set; }
        public DateTime RequestedDuration { get; set; } 
        public GatepassType GatepassType { get; set; }

        public VehicleDetails? VehicleDetails { get; set; } = new VehicleDetails();

        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
        public Guid? ApproverId { get; set; }
        public User? Approver { get; set;  }
        public DateTime? ApprovalDate { get; set; }
        public string? RejectionReason { get; set; } = string.Empty;
        public DateTime LastModifiedDate { get; set; }

    }
}
