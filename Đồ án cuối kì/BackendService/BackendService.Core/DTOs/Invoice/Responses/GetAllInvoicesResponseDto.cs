namespace BackendService.Core.DTOs.Invoice.Responses
{
    public class GetAllInvoicesResponseDto
    {
        public Guid InvoiceId { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public string? UserEmail { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public Guid? CustomerId { get; set; }
    }
}
