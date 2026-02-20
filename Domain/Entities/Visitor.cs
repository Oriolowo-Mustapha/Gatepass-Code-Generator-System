using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Visitor : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ContactNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public IdType IdType { get; set; }
        public int IdNumber { get; set; }
        public bool BlackListStatus { get; set; } = false;  
        public string BlackListReason { get; set; } = string.Empty;
        public Guid BlackListedBy { get; set; }
        public User BlackListedByUser { get; set; } = new User();
        public DateTime BlackListedDate { get; set; } 
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}
