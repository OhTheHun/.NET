using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Model.Common;

namespace BackendService.Mapping
{
    public static class CategoryToGetListCategoryAsync
    {
        public static GetListCategoryResponseDto Transform( Category category)
        {
            return new GetListCategoryResponseDto
            {
                Id = category.Id,
                TenDanhMuc = category.TenDanhMuc,
                Description = category.Description,
                ParentId = category.ParentId,
            };
        }
    }
}
