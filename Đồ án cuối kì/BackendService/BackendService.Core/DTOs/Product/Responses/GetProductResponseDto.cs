using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Core.DTOs.Product.Responses
{
    public class GetProductResponseDto: BaseProductDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId  { get; set; }
    }
}
