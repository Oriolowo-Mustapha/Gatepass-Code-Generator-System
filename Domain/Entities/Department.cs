using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Department : BaseEntity
    {
        public string DepartmentName { get; set;  } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public string HeadOfDepartment { get; set; } = string.Empty;
    }
}
