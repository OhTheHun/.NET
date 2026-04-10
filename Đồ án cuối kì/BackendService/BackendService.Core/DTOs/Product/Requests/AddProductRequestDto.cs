using BackendService.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Core.DTOs.Product.Requests
{
    public class AddProductRequestDto: BaseProductDto
    {
        public Guid CategoryId { get; set; }
        public Guid SupplierId { get; set; }
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public ProductEnum Status { get; set; }

    }
}
