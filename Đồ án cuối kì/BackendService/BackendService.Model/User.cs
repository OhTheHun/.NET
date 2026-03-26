using BeeExamPro.BackendService.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Model
{
    public class User: BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } 
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
