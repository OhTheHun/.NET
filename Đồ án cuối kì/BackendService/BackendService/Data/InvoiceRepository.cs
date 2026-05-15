using BackendService.Data.DataContext;
using BackendService.Data.Interface;
using BackendService.Model;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data
{
    public class InvoiceRepository(PostgresDbContext context) : IInvoiceRepository
    {
        private readonly PostgresDbContext _context = context;

        public async Task<List<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Where(i => i.DeleteFlag == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Invoice>> GetInvoicesByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Where(i => i.CustomerId == customerId && i.DeleteFlag == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.DeleteFlag == false, cancellationToken);
        }

        public async Task<List<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken)
        {
            return await _context.InvoiceItems
                .Where(ii => ii.InvoiceId == invoiceId && ii.DeleteFlag == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<Guid> productIds, CancellationToken cancellationToken)
        {
            return await _context.Products
                .Where(p => productIds.Contains(p.Id) && p.DeleteFlag == false)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(cancellationToken);
            return invoice;
        }

        public async Task<InvoiceItem[]> CreateListInvoiceItemAsync(InvoiceItem[] invoiceItems, CancellationToken cancellationToken)
        {
            _context.InvoiceItems.AddRange(invoiceItems);
            await _context.SaveChangesAsync(cancellationToken);
            return invoiceItems;
        }

        public async Task<List<Invoice>> GetOrdersForApprovalAsync(CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Where(i => i.DeleteFlag == false && i.Status != Model.Enums.InvoiceEnum.Canceled && i.Status != Model.Enums.InvoiceEnum.Completed)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Invoice>> GetProcessedOrdersByActorAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Where(i => i.DeleteFlag == false && 
                           i.UpdatedBy == userId && 
                           (i.Status == Model.Enums.InvoiceEnum.Completed || i.Status == Model.Enums.InvoiceEnum.Canceled))
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Invoice>> GetInvoicesByStatusAsync(Model.Enums.InvoiceEnum status, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Where(i => i.DeleteFlag == false && i.Status == status)
                .ToListAsync(cancellationToken);
        }
    }
}
