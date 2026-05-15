using BackendService.Model;

namespace BackendService.Data.Interface
{
    public interface IInvoiceRepository
    {
        Task<List<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken);
        Task<List<Invoice>> GetInvoicesByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
        Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken);
        Task<List<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken);
        Task<List<Product>> GetProductsByIdsAsync(List<Guid> productIds, CancellationToken cancellationToken);
        Task UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);
        Task <Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);
        Task<InvoiceItem[]> CreateListInvoiceItemAsync(InvoiceItem[] invoiceItems, CancellationToken cancellationToken);
        Task<List<Invoice>> GetOrdersForApprovalAsync(CancellationToken cancellationToken);
        Task<List<Invoice>> GetProcessedOrdersByActorAsync(string userId, CancellationToken cancellationToken);
        Task<List<Invoice>> GetInvoicesByStatusAsync(Model.Enums.InvoiceEnum status, CancellationToken cancellationToken);
    }
}
