using BackendService.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Core.DTOs.Product.Responses
{
    public class GetDetailProductResponseDto: BaseProductDto
    {
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public ProductEnum Status { get; set; }
    }
}
