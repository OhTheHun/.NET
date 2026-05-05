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
    }
}
