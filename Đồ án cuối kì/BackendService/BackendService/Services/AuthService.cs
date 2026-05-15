using BackendService.Configuration;
using BackendService.Core.DTOs.User.Requests;
using BackendService.Core.DTOs.User.Responses;
using BackendService.Data.Interface;
using BackendService.Model;
using BackendService.Services.Interface;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
namespace BackendService.Services
{
    public class AuthService(IUserRepository userRepository, IOptions<ConfigOptions> configOptions, IPasswordHasherService passwordHasherService, IEmailTemplateService emailTemplateService, IOtherService otherService, IServiceScopeFactory serviceScopeFactory): IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ConfigOptions _configOptions = configOptions.Value;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;
        private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;
        private readonly IOtherService _otherService = otherService;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                // tạo danh sách claim dữ liệu (iat: thời điểm token tạo, Chuyển thời gian sang dạng Unix Timestamp, đảm bảo về lỗi format
                new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                // tạo token riêng cho role
                new(ClaimTypes.Role, user.Role),
            };
            // tạo khóa ký token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configOptions.JwtConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // tạo token
            var token = new JwtSecurityToken(
                issuer: _configOptions.JwtConfig.Issuer,
                audience: _configOptions.JwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMilliseconds(Convert.ToDouble(_configOptions.JwtConfig.TokenValidityMiliSeconds)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken = default)
        {
            var userInDb = await _userRepository.GetUserByUsernameAsync(loginRequestDto.Username, cancellationToken);
            if (userInDb == null)
            {
                return null;
            }

            var isVerifiedPassword = _passwordHasherService.VerifyPassword(loginRequestDto.Password, userInDb.Password);
            if (!isVerifiedPassword)
            {
                return null;
            }

            return new LoginResponseDto
            {
                AccessToken = GenerateToken(userInDb),
                ExpireIn = _configOptions.JwtConfig.TokenValidityMiliSeconds,
                UserId = userInDb.Id,
                Role = userInDb.Role,
                FullName = userInDb.FullName

            };
        }
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                return false;
            }

            // Generate a 6 digit code
            string code = _otherService.GenerateResetPasswordCode();
            user.ResetPasswordCode = code;
            user.ResetPasswordExpiryTime = DateTime.UtcNow.AddMinutes(15);
            
            await _userRepository.UpdateUserAsync(user, cancellationToken);

            TaskSendResetPasswordEmail(user.Email, user.FullName, code, cancellationToken);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
            if (user == null || user.ResetPasswordCode != request.Code)
            {
                return false;
            }

            if (user.ResetPasswordExpiryTime == null || user.ResetPasswordExpiryTime < DateTime.UtcNow)
            {
                return false;
            }

            user.Password = _passwordHasherService.Hash(request.NewPassword);
            user.ResetPasswordCode = null;
            user.ResetPasswordExpiryTime = null;

            await _userRepository.UpdateUserAsync(user, cancellationToken);

            return true;
        }

        public void TaskSendResetPasswordEmail(string receiver, string? fullName, string code, CancellationToken cancellationToken)
        {
            var sender = _configOptions.EmailOptions.Sender;
            string subject = "MABIXI - Reset Your Password";

            string htmlBody = _emailTemplateService
               .GetEmailTemplate(Model.Enums.EmailTemplateType.ResetPassword)
               .Replace("{{FullName}}", fullName ?? receiver)
               .Replace("{{ResetCode}}", code);

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
