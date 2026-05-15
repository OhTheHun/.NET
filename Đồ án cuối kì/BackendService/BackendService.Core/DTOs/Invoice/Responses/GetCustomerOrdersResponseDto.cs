namespace BackendService.Core.DTOs.Invoice.Responses
{
    public class GetCustomerOrdersResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public List<InvoiceItemResponseDto> Items { get; set; } = new();
    }
}
