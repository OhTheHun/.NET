using BackendService.Core.DTOs.DonViTinh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendService.Core.DTOs.Product.Responses
{
    public class GetListByCategoryIdResponseDto: BaseProductDto
    {

        public Guid Id { get; set; }
        public BaseDonViTinhDto DonViTinh { get; set; } = new BaseDonViTinhDto();
    }
}
