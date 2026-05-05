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
    public class InvoiceController(IOptions<ConfigOptions> options, IInvoiceService invoiceService) : BackendBaseController(options)
    {
        private readonly IInvoiceService _invoiceService = invoiceService;

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
        public async Task<ActionResult> CancelOrderAsync([FromRoute] Guid invoiceId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _invoiceService.CancelOrderAsync(invoiceId, cancellationToken);
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
    }
}
