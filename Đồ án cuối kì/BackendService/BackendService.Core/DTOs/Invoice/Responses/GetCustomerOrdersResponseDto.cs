namespace BackendService.Core.DTOs.Invoice.Responses
{
    public class GetCustomerOrdersResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public List<InvoiceItemResponseDto> Items { get; set; } = new();
    }
}
