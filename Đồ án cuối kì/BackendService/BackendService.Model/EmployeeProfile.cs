using BeeExamPro.BackendService.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Model
{
    public class EmployeeProfile: BaseEntity
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Identify { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string Image { get; set; } = string.Empty;
    }
}
