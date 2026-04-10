using BackendService.Core.DTOs.Product.Requests;
using BackendService.Core.DTOs.Product.Responses;
using BackendService.Data.Interface;
using BackendService.Mapping;
using BackendService.Services.Interface;

namespace BackendService.Services
{
    public class ProductService(IProductRepository productRepository): IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<AddProductResponseDto> AddProductAsync(AddProductRequestDto request, string actor, CancellationToken cancellationToken)
        {
            var productInDb = await _productRepository.GetByNameAsync(request.ProductName, cancellationToken);
            if (productInDb != null)
            {
                throw new Exception("Product already exists");
            }
            var product = AddProductRequestDtoToProduct.Transform(request, actor);
            await _productRepository.CreateAsync(product, cancellationToken);
            var mapped = ProductToAddProductResponseDto.Transform(product);
            return mapped;
        }

        public async Task<GetProductResponseDto[]> GetListProductAsync(string? keyword, CancellationToken cancellationToken)
        {
            var productsInDb = await _productRepository.GetListAysnc(keyword, cancellationToken);
            var mapped = productsInDb.Select(ProductToGetProductResponseDto.Transform).ToArray();
            return mapped;

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
    }
}
