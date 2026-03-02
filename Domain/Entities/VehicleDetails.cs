using System;

namespace Domain.Entities
{
    public class VehicleDetails : BaseEntity
    {
        public Guid GatePassRequestID { get; set; }
        public GatepassRequest GatepassRequest { get; set; } = null!;
        public string? PlateNumber { get; set; }
    }
}
