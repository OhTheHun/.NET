using BackendService.Data.Interface;
using BackendService.Core.DTOs.Invoice.Requests;
using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Mapping;
using BackendService.Model;
using BackendService.Model.Enums;
using BackendService.Services.Interface;

namespace BackendService.Services
{
    public class InvoiceService(IInvoiceRepository invoiceRepository) : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;

        public async Task<List<GetAllInvoicesResponseDto>> GetCustomerOrdersAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var customerInvoices = await _invoiceRepository.GetInvoicesByCustomerIdAsync(customerId, cancellationToken);
            var resultList = new List<GetAllInvoicesResponseDto>();

            foreach (var invoice in customerInvoices)
            {
                var invoiceItems = await _invoiceRepository.GetInvoiceItemsByInvoiceIdAsync(invoice.Id, cancellationToken);
                var productIds = invoiceItems.Select(item => item.ProductId).Distinct().ToList();
                var productsInInvoice = await _invoiceRepository.GetProductsByIdsAsync(productIds, cancellationToken);
                
                resultList.Add(InvoiceToGetAllInvoicesResponseDto.transform(invoice, invoiceItems, productsInInvoice));
            }

            return resultList;
        }

        public async Task<GetCustomerOrdersResponseDto> GetInvoiceDetailAsync(Guid invoiceId, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId, cancellationToken);
            if (invoice == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            var invoiceItems = await _invoiceRepository.GetInvoiceItemsByInvoiceIdAsync(invoice.Id, cancellationToken);
            var productIds = invoiceItems.Select(item => item.ProductId).Distinct().ToList();
            var productsInInvoice = await _invoiceRepository.GetProductsByIdsAsync(productIds, cancellationToken);

            return InvoiceToGetCustomerOrdersResponseDto.transform(invoice, invoiceItems, productsInInvoice);
        }

        public async Task<bool> CancelOrderAsync(Guid invoiceId, CancellationToken cancellationToken)
        {
            var existingInvoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId, cancellationToken);
            
            if (existingInvoice == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            if (existingInvoice.Status != InvoiceEnum.Confirmed && existingInvoice.Status != InvoiceEnum.Processing)
            {
                if (existingInvoice.Status == InvoiceEnum.Delivering)
                {
                    throw new Exception("Không thể hủy đơn hàng đang trong quá trình giao hàng.");
                }
                throw new Exception($"Không thể hủy đơn hàng với trạng thái hiện tại: {existingInvoice.Status}");
            }

            existingInvoice.Status = InvoiceEnum.Canceled;
            existingInvoice.UpdatedTime = DateTime.UtcNow;
            
            await _invoiceRepository.UpdateInvoiceAsync(existingInvoice, cancellationToken);
            
            return true;
        }
    }
}
