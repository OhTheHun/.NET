namespace BackendService.Core.DTOs.Invoice.Responses
{
    public class GetAllInvoicesResponseDto
    {
        public Guid InvoiceId { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
    }
}
