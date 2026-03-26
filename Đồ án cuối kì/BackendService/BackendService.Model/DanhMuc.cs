using BeeExamPro.BackendService.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Model.Common
{
    public class DanhMuc: BaseEntity
    {
        public string TenDanhMuc { get; set; } = string.Empty;
    }
}
