using BackendService.Configuration;
using BackendService.Core.DTOs.Invoice.Requests;
using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Services.Interface;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackendService.Controllers
{
    [Route("api/invoice")]
    [ApiController]
    public class InvoiceController(IOptions<ConfigOptions> options, IInvoiceService invoiceService, 
        IValidator<AddInvoiceItemRequestDto> addInvoiceItemRequestValidator, 
        IValidator<AddInvoiceRequestDto> addInvoiceRequestValidator,
        IValidator<GetProcessedOrdersRequestDto> getProcessedOrdersRequestValidator,
        IValidator<FilterInvoiceRequestDto> filterInvoiceRequestValidator) : BackendBaseController(options)
    {
        private readonly IInvoiceService _invoiceService = invoiceService;
        private readonly IValidator<AddInvoiceItemRequestDto> _addInvoiceItemRequestValidator = addInvoiceItemRequestValidator;
        private readonly IValidator<AddInvoiceRequestDto> _addInvoiceRequestValidator = addInvoiceRequestValidator;
        private readonly IValidator<GetProcessedOrdersRequestDto> _getProcessedOrdersRequestValidator = getProcessedOrdersRequestValidator;
        private readonly IValidator<FilterInvoiceRequestDto> _filterInvoiceRequestValidator = filterInvoiceRequestValidator;

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<GetAllInvoicesResponseDto>>> GetCustomerOrdersAsync(Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                var orders = await _invoiceService.GetCustomerOrdersAsync(customerId, cancellationToken);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetCustomerOrdersResponseDto>> GetInvoiceDetailAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var detail = await _invoiceService.GetInvoiceDetailAsync(id, cancellationToken);
                return Ok(detail);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("cancel/{invoiceId}")]
        public async Task<ActionResult> CancelOrderAsync([FromRoute] Guid invoiceId, [FromQuery] string userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _invoiceService.CancelOrderAsync(invoiceId, userId, cancellationToken);
                if (result)
                {
                    return Ok(new { message = "Order cancelled successfully." });
                }
                return BadRequest(new { error = "Failed to cancel order." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("confirm-payment/{invoiceId}")]
        public async Task<ActionResult> ConfirmPaymentAsync([FromRoute] Guid invoiceId, [FromQuery] string userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _invoiceService.ConfirmPaymentAsync(invoiceId, userId, cancellationToken);
                if (result)
                {
                    return Ok(new { message = "Payment confirmed and inventory updated." });
                }
                return BadRequest(new { error = "Failed to confirm payment." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("create")]
        public async Task<ActionResult<AddInvoiceResponseDto>> CreateInvoiceAsync([FromBody] AddInvoiceRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _addInvoiceRequestValidator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { error = validationResult.Errors.Select(e => e.ErrorMessage) });
                }
                var result = await _invoiceService.CreateInvoiceAsync(request, "actor", cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("add-list")]
        public async Task<ActionResult> AddInvoiceItemsAsync([FromBody] AddInvoiceItemRequestDto[] request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResults = await Task.WhenAll(request.Select(r => _addInvoiceItemRequestValidator.ValidateAsync(r, cancellationToken)));
                var invalidResults = validationResults.Where(r => !r.IsValid).ToList();
                if (invalidResults.Any())
                {
                    return BadRequest(new { error = invalidResults.SelectMany(r => r.Errors).Select(e => e.ErrorMessage) });
                }
                var result = await _invoiceService.AddListInvoiceItemAsync(request, "actor", cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("admin/list")]
        public async Task<ActionResult<AdminOrdersPageDto>> GetAdminOrdersPageAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _invoiceService.GetAdminOrdersPageAsync(cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("update-processing/{invoiceId}")]
        public async Task<ActionResult> UpdateToProcessingAsync([FromRoute] Guid invoiceId, [FromQuery] string userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _invoiceService.UpdateToProcessingAsync(invoiceId, userId, cancellationToken);
                if (result) return Ok(new { message = "Cập nhật sang trạng thái Đang xử lý thành công." });
                return BadRequest(new { error = "Cập nhật thất bại." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("update-delivering/{invoiceId}")]
        public async Task<ActionResult> UpdateToDeliveringAsync([FromRoute] Guid invoiceId, [FromQuery] string userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _invoiceService.UpdateToDeliveringAsync(invoiceId, userId, cancellationToken);
                if (result) return Ok(new { message = "Cập nhật sang trạng thái Đang giao hàng thành công." });
                return BadRequest(new { error = "Cập nhật thất bại." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("admin/approval")]
        public async Task<ActionResult<List<AdminOrderDto>>> GetOrdersForApprovalAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _invoiceService.GetOrdersForApprovalAsync(cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("processed-orders/{userId}")]
        public async Task<ActionResult<List<GetActorProcessedOrdersResponseDto>>> GetProcessedOrdersByActorAsync([FromRoute] string userId, CancellationToken cancellationToken)
        {
            try
            {
                var request = new GetProcessedOrdersRequestDto { UserId = userId };
                var validationResult = await _getProcessedOrdersRequestValidator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { error = validationResult.Errors.Select(e => e.ErrorMessage) });
                }
                var result = await _invoiceService.GetProcessedOrdersByActorAsync(userId, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<GetAllInvoicesResponseDto>>> GetAllInvoicesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _invoiceService.GetAllInvoicesAsync(cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<GetAllInvoicesResponseDto>>> FilterInvoicesByStatusAsync([FromQuery] FilterInvoiceRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _filterInvoiceRequestValidator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { error = validationResult.Errors.Select(e => e.ErrorMessage) });
                }
                var result = await _invoiceService.FilterInvoicesByStatusAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}