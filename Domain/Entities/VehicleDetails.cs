using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class VehicleDetails : BaseEntity
    {
        public Guid GatePassRequestID { get; set; }
        public Gatepass Gatepass { get; set; } = new Gatepass();
        public string PlateNumber { get; set; } = string.Empty;
        public VehicleType VehicleType { get; set; }
        public string VehicleColor { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
    }
}
