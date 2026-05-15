using BackendService.Configuration;
using BackendService.Constants;
using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Core.DTOs.Product.Requests;
using BackendService.Core.DTOs.Product.Responses;
using BackendService.Core.DTOs.User.Requests;
using BackendService.Services.Interface;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackendService.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProductController(
        IOptions<ConfigOptions> options, 
        IProductService productService, 
        IValidator<AddProductRequestDto> addProductRequestValidator,
        IValidator<UpdateProductRequestDto> updateProductRequestValidator) : BackendBaseController(options)
    {

        private readonly ConfigOptions _configOptions = options.Value;
        private readonly IProductService _productService = productService;
        private readonly IValidator<AddProductRequestDto> _addProductRequestValidator = addProductRequestValidator;
        private readonly IValidator<UpdateProductRequestDto> _updateProductRequestValidator = updateProductRequestValidator;

        [HttpGet("product/{productId:guid}")]
        public async Task<ActionResult<GetDetailProductResponseDto>> GetProductByIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken)
        {
            try
            {
                var productResponse = await _productService.GetProductByIdAsync(productId, cancellationToken);
                if (productResponse == null)
                {
                    return NotFound(new { error = "Detail product not found" });
                }
                return Ok(productResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        [HttpGet("product/list")]
        public async Task<ActionResult<List<GetProductResponseDto>>> GetListProductsAsync([FromQuery] string? keyword, CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productService.GetListProductAsync(keyword, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpGet("admin/product/list")]
        public async Task<ActionResult<List<GetAdminProductResponseDto>>> GetAdminListProductsAsync(
            [FromQuery] string? keyword, 
            [FromQuery] Guid? categoryId, 
            [FromQuery] int? status, 
            CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productService.GetAdminListProductAsync(keyword, categoryId, status, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost("product/create")]
        public async Task<ActionResult<AddProductResponseDto>> CreateProductAsync([FromBody] AddProductRequestDto addProductRequestDto, CancellationToken cancellationToken)
        {
            try
            {
                var actor = Username;
                var validationResult = await _addProductRequestValidator.ValidateAsync(addProductRequestDto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }
                var createdProductId = await _productService.AddProductAsync(addProductRequestDto, actor, cancellationToken);
                return createdProductId;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPut("product/update")]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductRequestDto updateProductRequestDto, CancellationToken cancellationToken)
        {
            try
            {
                var actor = Username;
                var validationResult = await _updateProductRequestValidator.ValidateAsync(updateProductRequestDto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }
                await _productService.UpdateProductAsync(updateProductRequestDto, actor, cancellationToken);
                return Ok(new { message = "Product updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpDelete("product/delete/{productId:guid}")]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] Guid productId, CancellationToken cancellationToken)
        {
            try
            {
                var actor = Username;
                await _productService.DeleteProductAsync(productId, actor, cancellationToken);
                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpGet("product/list/{categoryId:guid}")]
        public async Task<ActionResult<List<GetListByCategoryIdResponseDto>>> GetListByCategoryIdAsync([FromRoute] Guid categoryId, CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productService.GetListByCategoryIdAsync(categoryId, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}