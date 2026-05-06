using BackendService.Core.DTOs.DonViTinh;
using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Core.DTOs.Product.Responses;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class ProductToGetListByCategoryIdResponseDto
    {
        public static GetListByCategoryIdResponseDto Transform(Product product)
        {
            return new GetListByCategoryIdResponseDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                Image_Url = product.Image_Url,
                DonViTinh = new BaseDonViTinhDto
                {
                    Ten = product.DonViTinh.TenDonViTinh
                }
            };
        }
    }
}
