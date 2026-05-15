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
                DonViTinhId = dto.DonViTinhId,
                ProductName = dto.ProductName,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice,
                Cost = dto.Cost,
                SKU = dto.SKU,
                Description = dto.Description,
                Image_Url = dto.Image_Url,
                Status = dto.Status,
                CreatedBy = actor,
                CreatedTime = DateTime.UtcNow,
                UpdatedBy = actor,
                UpdatedTime = DateTime.UtcNow,
                DeleteFlag = false
            };
        }
    }
}
