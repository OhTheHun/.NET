using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Core.DTOs.Product.Requests;
using BackendService.Core.DTOs.Product.Responses;
using BackendService.Data.Interface;
using BackendService.Mapping;
using BackendService.Model;
using BackendService.Services.Interface;

namespace BackendService.Services
{
    public class ProductService(IProductRepository productRepository, IInventoryRepository inventoryRepository) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IInventoryRepository _inventoryRepository = inventoryRepository;

        public async Task<AddProductResponseDto> AddProductAsync(AddProductRequestDto request, string actor, CancellationToken cancellationToken)
        {
            var productInDb = await _productRepository.GetByNameAsync(request.ProductName, cancellationToken);
            if (productInDb != null)
            {
                throw new Exception("Product already exists");
            }
            var product = AddProductRequestDtoToProduct.Transform(request, actor);
            await _productRepository.CreateAsync(product, cancellationToken);

            var inventory = ProductToInventory.Transform(product, actor);
            await _inventoryRepository.CreateAsync(inventory, cancellationToken);

            var mapped = ProductToAddProductResponseDto.Transform(product);
            return mapped;
        }

        public async Task<GetListCategoryResponseDto[]> GetListCategoryAsync(CancellationToken cancellationToken)
        {
            var categoriesInDb = await _productRepository.GetListCategoryAsync(cancellationToken);
            var mapped = categoriesInDb.Select(CategoryToGetListCategoryAsync.Transform).ToArray();
            return mapped;
        }

        public async Task<GetProductResponseDto[]> GetListProductAsync(string? keyword, CancellationToken cancellationToken)
        {
            var productsInDb = await _productRepository.GetListAysnc(keyword, cancellationToken);
            var mapped = productsInDb.Select(ProductToGetProductResponseDto.Transform).ToArray();
            return mapped;

        }

        public async Task<GetAdminProductResponseDto[]> GetAdminListProductAsync(string? keyword, Guid? categoryId, int? status, CancellationToken cancellationToken)
        {
            var productsInDb = await _productRepository.GetAdminListAsync(keyword, categoryId, status, cancellationToken);
            var resultList = new List<GetAdminProductResponseDto>();
            foreach (var product in productsInDb)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(product.Id, cancellationToken);
                resultList.Add(ProductToGetAdminProductResponseDto.Transform(product, inventory));
            }
            return resultList.ToArray();
        }

        public async Task UpdateProductAsync(UpdateProductRequestDto request, string actor, CancellationToken cancellationToken)
        {
            var productInDb = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (productInDb == null)
            {
                throw new Exception("Product not found");
            }
            var product = UpdateProductRequestDtoToProduct.Transform(request, productInDb, actor);
            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        public async Task DeleteProductAsync(Guid productId, string actor, CancellationToken cancellationToken)
        {
            await _productRepository.SoftDeleteAsync(productId, actor, cancellationToken);
        }

        public async Task<GetDetailProductResponseDto> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            var productInDb = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (productInDb == null)
            {
                throw new Exception("Product not found");
            }
            var mapped = ProductToGetDetailProductResponseDto.Transform(productInDb);
            return mapped;
        }
        public async Task<GetListByCategoryIdResponseDto[]> GetListByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            var productsInDb = await _productRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
            var mapped = productsInDb.Select(ProductToGetListByCategoryIdResponseDto.Transform).ToArray();
            return mapped;
        }
    }
}
