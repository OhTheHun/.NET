using BackendService.Core.DTOs.User.Requests;
using BackendService.Core.DTOs.User.Responses;
using BackendService.Model;

namespace BackendService.Services.Interface
{
    public interface IUserService
    {
        Task<RegisterUserResponseDto> RegisterUserAsync(RegisterUserRequestDto registerRequestDto, string actor, CancellationToken cancellationToken);
        Task<GetUserByIdResponseDto> GetUserByIdAsync (Guid userId, CancellationToken cancellationToken);
        Task<GetUserByIdResponseDto[]> GetListUserAsync(string? keyword, IReadOnlyList<string>? roles, CancellationToken cancellationToken);

    }
}
