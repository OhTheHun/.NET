using BackendService.Core.DTOs.User.Requests;
using BackendService.Core.DTOs.User.Responses;
using BackendService.Model;

namespace BackendService.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken = default);
        string GenerateToken(User user);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default);
    }
}
