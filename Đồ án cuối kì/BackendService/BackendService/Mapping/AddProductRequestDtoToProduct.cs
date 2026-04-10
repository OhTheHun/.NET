using BackendService.Core.DTOs.Product.Requests;
using BackendService.Model;
using BackendService.Model.Enums;

namespace BackendService.Mapping
{
    public static class AddProductRequestDtoToProduct
    {
        public static Product Transform(AddProductRequestDto dto, string actor)
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                ProductName = dto.ProductName,
                Price = dto.Price,
                DiscountPrice = 0,
                Cost = dto.Cost,
                Description = dto.Description,
                Image_Url = dto.Image_Url,
                Status = (int)ProductEnum.Draft,
                CreatedBy = actor,
                CreatedTime = DateTime.UtcNow,
                UpdatedBy = actor,
                UpdatedTime = DateTime.UtcNow,
                DeleteFlag = false
            };
        }
    }
}
