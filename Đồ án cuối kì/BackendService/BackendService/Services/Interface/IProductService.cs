using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Core.DTOs.Product.Requests;
using BackendService.Core.DTOs.Product.Responses;

namespace BackendService.Services.Interface
{
    public interface IProductService
    {
        Task<GetDetailProductResponseDto> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);
        Task<GetProductResponseDto[]> GetListProductAsync(string? keyword, CancellationToken cancellationToken);
        Task<GetAdminProductResponseDto[]> GetAdminListProductAsync(string? keyword, Guid? categoryId, int? status, CancellationToken cancellationToken);
        Task<AddProductResponseDto> AddProductAsync(AddProductRequestDto request, string actor, CancellationToken cancellationToken);
        Task UpdateProductAsync(UpdateProductRequestDto request, string actor, CancellationToken cancellationToken);
        Task DeleteProductAsync(Guid productId, string actor, CancellationToken cancellationToken);
        Task<GetListCategoryResponseDto[]> GetListCategoryAsync(CancellationToken cancellationToken);
        Task<GetListByCategoryIdResponseDto[]> GetListByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken);
    }
}
