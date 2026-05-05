using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class InvoiceToGetAllInvoicesResponseDto
    {
        public static GetAllInvoicesResponseDto transform(Invoice invoice, List<InvoiceItem> invoiceItems, List<Product> products)
        {
            return new GetAllInvoicesResponseDto
            {
                Code = invoice.Code,
                TotalAmount = invoice.TotalAmount,
                Status = invoice.Status.ToString(),
                CreatedTime = invoice.CreatedTime,
                InvoiceId = invoice.Id

            };
        }
    }
}
