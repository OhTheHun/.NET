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

        public async Task<User[]> GetCustomersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Where(x => x.Role == "Customer" && !x.DeleteFlag)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<(User User, EmployeeProfile? Profile)[]> GetEmployeesAsync(string? keyword, CancellationToken cancellationToken = default)
        {
            var query = from user in _dbContext.Users
                        join profile in _dbContext.EmployeeProfiles on user.Id equals profile.UserId into profiles
                        from profile in profiles.DefaultIfEmpty()
                        where !user.DeleteFlag && (user.Role == "Seller" || user.Role == "WareHouseManager")
                        select new { User = user, Profile = profile };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.ToLower();
                query = query.Where(x =>
                    (x.User.FullName != null && x.User.FullName.ToLower().Contains(kw)) ||
                    x.User.Email.ToLower().Contains(kw) ||
                    (x.User.Phone != null && x.User.Phone.Contains(kw)) ||
                    (x.Profile != null && x.Profile.Identify.Contains(kw))
                );
            }

            var result = await query.ToListAsync(cancellationToken);
            return result.Select(x => (x.User, x.Profile)).ToArray();
        }

        public async Task AddEmployeeAsync(User user, EmployeeProfile profile, CancellationToken cancellationToken = default)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _dbContext.Users.AddAsync(user, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                profile.UserId = user.Id;
                await _dbContext.EmployeeProfiles.AddAsync(profile, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task UpdateEmployeeAsync(User user, EmployeeProfile profile, CancellationToken cancellationToken = default)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _dbContext.Users.Update(user);
                
                var existingProfile = await _dbContext.EmployeeProfiles
                    .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);

                if (existingProfile != null)
                {
                    existingProfile.Identify = profile.Identify;
                    existingProfile.Salary = profile.Salary;
                    existingProfile.Date = profile.Date;
                    existingProfile.UpdatedBy = profile.UpdatedBy;
                    existingProfile.UpdatedTime = profile.UpdatedTime;
                    _dbContext.EmployeeProfiles.Update(existingProfile);
                }
                else
                {
                    profile.UserId = user.Id;
                    await _dbContext.EmployeeProfiles.AddAsync(profile, cancellationToken);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task SoftDeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.DeleteFlag = true;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email && user.DeleteFlag == false, cancellationToken);
        }

        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
