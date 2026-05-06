using BackendService.Core.DTOs.Product.Responses;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class ProductToAddProductResponseDto
    {
        public static AddProductResponseDto Transform(Product product)
        {
            return new AddProductResponseDto
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                DonViTinhId = product.DonViTinhId,
                SupplierId  = product.SupplierId,
                ProductName = product.ProductName,
                DiscountPrice = product.DiscountPrice,
                Price = product.Price,
                Cost = product.Cost,
                Image_Url = product.Image_Url

                
            };
        }
    }
}
