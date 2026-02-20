using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public string Permissions { get; set; } = string.Empty; 
    }
}
