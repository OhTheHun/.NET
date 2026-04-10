using BeeExamPro.BackendService.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Model
{
    public class Supplier: BaseEntity
    {
        public string SupplierName { get; set; } = string.Empty;
        public int PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Identify { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
