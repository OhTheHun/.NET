using BackendService.Data.Interface;
using BackendService.Core.DTOs.Invoice.Requests;
using BackendService.Core.DTOs.Invoice.Responses;
using BackendService.Mapping;
using BackendService.Model;
using BackendService.Model.Enums;
using BackendService.Services.Interface;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using BackendService.Configuration;

namespace BackendService.Services
{
    public class InvoiceService(IInvoiceRepository invoiceRepository, IOtherService OtherService, IInventoryRepository inventoryRepository, IUserRepository userRepository, IServiceScopeFactory serviceScopeFactory, IEmailTemplateService emailTemplateService, IOptions<ConfigOptions> configOptions) : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
        private readonly IOtherService _otherService = OtherService;
        private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;
        private readonly ConfigOptions _configOptions = configOptions.Value;

        public async Task<List<GetAllInvoicesResponseDto>> GetCustomerOrdersAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var customerInvoices = await _invoiceRepository.GetInvoicesByCustomerIdAsync(customerId, cancellationToken);
            return await MapInvoicesToResponse(customerInvoices, cancellationToken);
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

        public async Task<bool> CancelOrderAsync(Guid invoiceId, string userId, CancellationToken cancellationToken)
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
            existingInvoice.UpdatedBy = userId;
            
            await _invoiceRepository.UpdateInvoiceAsync(existingInvoice, cancellationToken);
            
            // Send cancellation email if customer exists
            if (existingInvoice.CustomerId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(existingInvoice.CustomerId.Value, cancellationToken);
                if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                {
                    TaskSendCancelOrderEmail(user.Email, user.FullName ?? user.Email, existingInvoice.Code, "Hàng đã hết", cancellationToken);
                }
            }

            return true;
        }

        public async Task<bool> ConfirmPaymentAsync(Guid invoiceId, string userId, CancellationToken cancellationToken)
        {
            var existingInvoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId, cancellationToken);

            if (existingInvoice == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            if (existingInvoice.Status != InvoiceEnum.Delivering)
            {
                throw new Exception($"Đơn hàng không ở trạng thái có thể xác nhận thanh toán. Trạng thái hiện tại: {existingInvoice.Status}");
            }

            var invoiceItems = await _invoiceRepository.GetInvoiceItemsByInvoiceIdAsync(invoiceId, cancellationToken);

            foreach (var item in invoiceItems)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(item.ProductId, cancellationToken);
                if (inventory != null)
                {
                    if (inventory.quantity < item.Quantity)
                    {
                         throw new Exception($"Sản phẩm với ID {item.ProductId} không đủ số lượng trong kho.");
                    }

                    inventory.quantity -= item.Quantity;
                    inventory.UpdatedTime = DateTime.UtcNow;
                    await _inventoryRepository.UpdateAsync(inventory, cancellationToken);
                }
                else
                {
                    throw new Exception($"Sản phẩm với ID {item.ProductId} không có thông tin tồn kho.");
                }
            }

            existingInvoice.Status = InvoiceEnum.Completed;
            existingInvoice.UpdatedTime = DateTime.UtcNow;
            existingInvoice.UpdatedBy = userId;

            await _invoiceRepository.UpdateInvoiceAsync(existingInvoice, cancellationToken);

            return true;
        }

        public async Task<AddInvoiceResponseDto> CreateInvoiceAsync(AddInvoiceRequestDto request, string actor, CancellationToken cancellationToken)
        {
            var invoice = AddInvoiceRequestDtoToInvoice.Transform(request, _otherService.GenerateRandomCode(), actor);
            await _invoiceRepository.CreateInvoiceAsync(invoice, cancellationToken);
            return InvoiceToAddInvoiceResponseDto.Transform(invoice);

        }

        public async Task<bool> AddListInvoiceItemAsync(AddInvoiceItemRequestDto[] request, string actor, CancellationToken cancellationToken)
        {
            var invoiceItems = request.Select(AddInvoiceItemRequestDtoToInvoiceItem.Transform).ToArray();
            await _invoiceRepository.CreateListInvoiceItemAsync(invoiceItems, cancellationToken);
            return true;
        }

        public async Task<AdminOrdersPageDto> GetAdminOrdersPageAsync(CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync(cancellationToken);
            return InvoiceToAdminOrdersPageDto.Transform(invoices);
        }

        public async Task<bool> UpdateToProcessingAsync(Guid invoiceId, string userId, CancellationToken cancellationToken)
        {
            var existingInvoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId, cancellationToken);
            if (existingInvoice == null) throw new Exception("Không tìm thấy đơn hàng.");

            existingInvoice.Status = InvoiceEnum.Processing;
            existingInvoice.UpdatedTime = DateTime.UtcNow;
            existingInvoice.UpdatedBy = userId;
            await _invoiceRepository.UpdateInvoiceAsync(existingInvoice, cancellationToken);
            return true;
        }

        public async Task<bool> UpdateToDeliveringAsync(Guid invoiceId, string userId, CancellationToken cancellationToken)
        {
            var existingInvoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId, cancellationToken);
            if (existingInvoice == null) throw new Exception("Không tìm thấy đơn hàng.");

            existingInvoice.Status = InvoiceEnum.Delivering;
            existingInvoice.UpdatedTime = DateTime.UtcNow;
            existingInvoice.UpdatedBy = userId;
            await _invoiceRepository.UpdateInvoiceAsync(existingInvoice, cancellationToken);
            return true;
        }
        public async Task<List<AdminOrderDto>> GetOrdersForApprovalAsync(CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetOrdersForApprovalAsync(cancellationToken);
            return invoices.OrderByDescending(i => i.CreatedTime).Select(i => new AdminOrderDto
            {
                Id = i.Id,
                Code = i.Code,
                CustomerName = string.IsNullOrEmpty(i.FullName) ? "Khách vãng lai" : i.FullName,
                CreatedTime = i.CreatedTime,
                TotalAmount = i.TotalAmount,
                Status = i.Status.ToString()
            }).ToList();
        }

        public async Task<List<GetActorProcessedOrdersResponseDto>> GetProcessedOrdersByActorAsync(string userId, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetProcessedOrdersByActorAsync(userId, cancellationToken);
            return InvoiceToGetActorProcessedOrdersResponseDto.transform(invoices);
        }

        public async Task<List<GetAllInvoicesResponseDto>> GetAllInvoicesAsync(CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync(cancellationToken);
            return await MapInvoicesToResponse(invoices, cancellationToken);
        }

        public async Task<List<GetAllInvoicesResponseDto>> FilterInvoicesByStatusAsync(FilterInvoiceRequestDto request, CancellationToken cancellationToken)
        {
            if (request.Status == null) return await GetAllInvoicesAsync(cancellationToken);
            
            var invoices = await _invoiceRepository.GetInvoicesByStatusAsync(request.Status.Value, cancellationToken);
            return await MapInvoicesToResponse(invoices, cancellationToken);
        }

        private async Task<List<GetAllInvoicesResponseDto>> MapInvoicesToResponse(List<Invoice> invoices, CancellationToken cancellationToken)
        {
            var customerIds = invoices.Where(i => i.CustomerId.HasValue).Select(i => i.CustomerId!.Value).Distinct().ToList();
            var users = new Dictionary<Guid, string?>();

            foreach (var id in customerIds)
            {
                var user = await _userRepository.GetByIdAsync(id, cancellationToken);
                users[id] = user?.Email;
            }

            return invoices.Select(invoice => 
            {
                string? email = null;
                if (invoice.CustomerId.HasValue)
                {
                    users.TryGetValue(invoice.CustomerId.Value, out email);
                }
                return InvoiceToGetAllInvoicesResponseDto.transform(invoice, email);
            }).ToList();
        }

        private void TaskSendCancelOrderEmail(string receiver, string fullName, string orderCode, string reason, CancellationToken cancellationToken)
        {
            var sender = _configOptions.EmailOptions.Sender;
            string subject = $"MABIXI - Thông báo hủy đơn hàng {orderCode}";

            string htmlBody = _emailTemplateService
               .GetEmailTemplate(Model.Enums.EmailTemplateType.CancelOrder)
               .Replace("{{FullName}}", fullName)
               .Replace("{{OrderCode}}", orderCode)
               .Replace("{{Reason}}", reason);

            var emailHistory = new EmailHistory
            {
                Subject = subject,
                Sender = sender.Email,
                Received = receiver,
                Body = htmlBody,
                EmailStatus = (int)Model.Enums.EmailStatus.Fail,
                CreatedBy = "System",
                UpdatedBy = "System"
            };

            _ = Task.Factory.StartNew(async () =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BackendService.Data.DataContext.PostgresDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    await emailService.SendAsync(receiver, subject, htmlBody, null, cancellationToken);
                    
                    emailHistory.EmailStatus = (int)Model.Enums.EmailStatus.Success;
                    await dbContext.EmailHistories.AddAsync(emailHistory, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }).ContinueWith(async t =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BackendService.Data.DataContext.PostgresDbContext>();
                    
                    emailHistory.EmailStatus = (int)Model.Enums.EmailStatus.Fail;
                    if (t.Exception != null)
                    {
                        var ex = t.Exception.Flatten().InnerExceptions.FirstOrDefault() ?? t.Exception;
                        emailHistory.Exceptions = ex.Message;
                    }
                    
                    await dbContext.EmailHistories.AddAsync(emailHistory, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
