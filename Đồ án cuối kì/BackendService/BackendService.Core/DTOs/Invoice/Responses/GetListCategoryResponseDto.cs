using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Core.DTOs.Invoice.Responses
{
    public class GetListCategoryResponseDto
    {
        public Guid Id { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
    }
}
