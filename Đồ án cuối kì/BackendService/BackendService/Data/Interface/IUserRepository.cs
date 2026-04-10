using BackendService.Model;

namespace BackendService.Data.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameAsync(string UserName, CancellationToken cancellationToken = default);
        Task<User> RegisterUserAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync (Guid userId, CancellationToken cancellationToken = default);
        Task<User[]> GetListUserAsync(string? keyword, IReadOnlyList<string>? roles, CancellationToken cancellationToken = default);

    }
}
