using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Core.DTOs.Product
{
    public class BaseProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public string Image_Url { get; set; } = string.Empty;
    }
}
