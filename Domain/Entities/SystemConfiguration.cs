using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class SystemConfiguration : BaseEntity
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
}
