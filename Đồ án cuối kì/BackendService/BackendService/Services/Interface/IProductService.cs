using BackendService.Core.DTOs.Product.Requests;
using BackendService.Core.DTOs.Product.Responses;

namespace BackendService.Services.Interface
{
    public interface IProductService
    {
        Task<GetDetailProductResponseDto> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);
        Task<GetProductResponseDto[]> GetListProductAsync(string? keyword, CancellationToken cancellationToken);
        Task<AddProductResponseDto> AddProductAsync(AddProductRequestDto request, string actor, CancellationToken cancellationToken);
    }
}
