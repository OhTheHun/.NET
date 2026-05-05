namespace BackendService.Core.DTOs.Invoice.Responses
{
    public class InvoiceItemResponseDto
    {
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
    }
}
