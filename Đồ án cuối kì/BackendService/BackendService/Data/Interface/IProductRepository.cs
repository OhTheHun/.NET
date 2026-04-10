using BackendService.Model;

namespace BackendService.Data.Interface
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync (Guid productId, CancellationToken cancellationToken);
        Task<Product[]> GetListAysnc(string? keyword, CancellationToken cancellationToken);
        Task<Product> CreateAsync(Product product, CancellationToken cancellationToken);
        Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken);
        Task<Product?> GetByNameAsync (string name, CancellationToken cancellationToken);
    }
}
