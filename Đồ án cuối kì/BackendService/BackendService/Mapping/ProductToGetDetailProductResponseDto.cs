using BackendService.Core.DTOs.Product.Responses;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class ProductToGetDetailProductResponseDto
    {
        public static GetDetailProductResponseDto Transform(Product product)
        {
            return new GetDetailProductResponseDto
            {
                ProductName = product.ProductName,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                Description = product.Description,
                Cost = product.Cost,
                Status = product.Status,
                Image_Url = product.Image_Url
            };
        }
    }
}
