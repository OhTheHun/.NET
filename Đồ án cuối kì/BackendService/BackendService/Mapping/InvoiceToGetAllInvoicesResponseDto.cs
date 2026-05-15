using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class InvoiceToGetAllInvoicesResponseDto
    {
        public static GetAllInvoicesResponseDto transform(Invoice invoice, string? userEmail)
        {
            return new GetAllInvoicesResponseDto
            {
                InvoiceId = invoice.Id,
                Code = invoice.Code,
                TotalAmount = invoice.TotalAmount,
                Status = invoice.Status.ToString(),
                CreatedTime = invoice.CreatedTime,
                UserEmail = userEmail,
                FullName = invoice.FullName,
                Phone = invoice.Phone,
                Address = invoice.Address,
                PaymentMethod = invoice.PaymentMethod,
                CustomerId = invoice.CustomerId
            };
        }

        public static GetAllInvoicesResponseDto transform(Invoice invoice, List<InvoiceItem> invoiceItems, List<Product> products)
        {
            return new GetAllInvoicesResponseDto
            {
                InvoiceId = invoice.Id,
                Code = invoice.Code,
                TotalAmount = invoice.TotalAmount,
                Status = invoice.Status.ToString(),
                CreatedTime = invoice.CreatedTime,
                FullName = invoice.FullName,
                Phone = invoice.Phone,
                Address = invoice.Address,
                PaymentMethod = invoice.PaymentMethod,
                CustomerId = invoice.CustomerId
            };
        }
    }
}
