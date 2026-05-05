using BackendService.Core.DTOs.Invoice.Requests;
using BackendService.Core.DTOs.Invoice.Responses;

namespace BackendService.Services.Interface
{
    public interface IInvoiceService
    {
        Task<List<GetAllInvoicesResponseDto>> GetCustomerOrdersAsync(Guid customerId, CancellationToken cancellationToken);
        Task<GetCustomerOrdersResponseDto> GetInvoiceDetailAsync(Guid invoiceId, CancellationToken cancellationToken);
        Task<bool> CancelOrderAsync(Guid invoiceId, CancellationToken cancellationToken);
    }
}
