using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class AccessPoint : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string LocationDescription { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
