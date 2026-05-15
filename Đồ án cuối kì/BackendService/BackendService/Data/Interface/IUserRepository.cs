using BackendService.Model;

namespace BackendService.Data.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameAsync(string UserName, CancellationToken cancellationToken = default);
        Task<User> RegisterUserAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync (Guid userId, CancellationToken cancellationToken = default);
        Task<User[]> GetListUserAsync(string? keyword, IReadOnlyList<string>? roles, CancellationToken cancellationToken = default);
        Task<User[]> GetCustomersAsync(CancellationToken cancellationToken = default);
        Task<(User User, EmployeeProfile? Profile)[]> GetEmployeesAsync(string? keyword, CancellationToken cancellationToken = default);
        Task AddEmployeeAsync(User user, EmployeeProfile profile, CancellationToken cancellationToken = default);
        Task UpdateEmployeeAsync(User user, EmployeeProfile profile, CancellationToken cancellationToken = default);
        Task SoftDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    }
}
