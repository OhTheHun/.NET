using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Model;

namespace BackendService.Mapping
{
    public static class InvoiceToGetCustomerOrdersResponseDto
    {
        public static GetCustomerOrdersResponseDto transform(Invoice invoice, List<InvoiceItem> invoiceItems, List<Product> products)
        {
            return new GetCustomerOrdersResponseDto
            {
                Code = invoice.Code,
                PaymentMethod = invoice.PaymentMethod,
                TotalAmount = invoice.TotalAmount,
                Status = invoice.Status.ToString(),
                CreatedTime = invoice.CreatedTime,
                Items = invoiceItems.Select(item =>
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    return new InvoiceItemResponseDto
                    {
                        Quantity = item.Quantity,
                        Total = item.Total,
                        ProductName = product?.ProductName ?? "Sản phẩm không xác định",
                        Price = product?.Price ?? 0,
                        DiscountPrice = product?.DiscountPrice ?? 0
                    };
                }).ToList()
            };
        }
    }
}
