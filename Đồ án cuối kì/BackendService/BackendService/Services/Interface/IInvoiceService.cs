using BackendService.Core.DTOs.Invoice.Requests;
using BackendService.Core.DTOs.Invoice.Responses;

namespace BackendService.Services.Interface
{
    public interface IInvoiceService
    {
        Task<List<GetAllInvoicesResponseDto>> GetCustomerOrdersAsync(Guid customerId, CancellationToken cancellationToken);
        Task<GetCustomerOrdersResponseDto> GetInvoiceDetailAsync(Guid invoiceId, CancellationToken cancellationToken);
        Task<bool> CancelOrderAsync(Guid invoiceId, string userId, CancellationToken cancellationToken);
        Task<AddInvoiceResponseDto> CreateInvoiceAsync(AddInvoiceRequestDto request, string actor, CancellationToken cancellationToken);
        Task<bool> AddListInvoiceItemAsync(AddInvoiceItemRequestDto[] request, string actor, CancellationToken cancellationToken);
        Task<bool> ConfirmPaymentAsync(Guid invoiceId, string userId, CancellationToken cancellationToken);
        Task<bool> UpdateToProcessingAsync(Guid invoiceId, string userId, CancellationToken cancellationToken);
        Task<bool> UpdateToDeliveringAsync(Guid invoiceId, string userId, CancellationToken cancellationToken);
        Task<AdminOrdersPageDto> GetAdminOrdersPageAsync(CancellationToken cancellationToken);
        Task<List<AdminOrderDto>> GetOrdersForApprovalAsync(CancellationToken cancellationToken);
        Task<List<GetActorProcessedOrdersResponseDto>> GetProcessedOrdersByActorAsync(string userId, CancellationToken cancellationToken);
        Task<List<GetAllInvoicesResponseDto>> GetAllInvoicesAsync(CancellationToken cancellationToken);
        Task<List<GetAllInvoicesResponseDto>> FilterInvoicesByStatusAsync(FilterInvoiceRequestDto request, CancellationToken cancellationToken);
    }
}
