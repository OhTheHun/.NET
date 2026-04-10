using BackendService.Data.DataContext;
using BackendService.Data.Interface;
using BackendService.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace BackendService.Data
{
    public class UserRepository(PostgresDbContext dbContext): IUserRepository
    {
        private readonly PostgresDbContext _dbContext = dbContext;

        public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
           return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId && !user.DeleteFlag, cancellationToken);
        }

        public async Task<User[]> GetListUserAsync(string? keyword, IReadOnlyList<string>? roles, CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = _dbContext.Users.AsNoTracking();

            if (roles is { Count: > 0 })
            {
                query = query.Where(x => roles.Contains(x.Role));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                   x.Email.ToLower().Contains(keyword.ToLower()));
            }

            query = query
                .Where(x => !x.DeleteFlag);
            return await query.ToArrayAsync(cancellationToken);
        }

        public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == username && user.DeleteFlag == false, cancellationToken);
        }
        public async Task<User> RegisterUserAsync(User user, CancellationToken cancellationToken = default)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return user;
        }

    }
}
