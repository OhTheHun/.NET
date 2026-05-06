using BackendService.Data.DataContext;
using BackendService.Data.Interface;
using BackendService.Model;
using BackendService.Model.Common;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BackendService.Data
{
    public class ProductRepository(PostgresDbContext dbContext) : IProductRepository
    {
        private readonly PostgresDbContext _dbContext = dbContext;

        public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(product);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return product;

        }

        public async Task<Product[]> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .Where(p => p.CategoryId == categoryId && !p.DeleteFlag)
                .Include(p => p.DonViTinh)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            return await _dbContext.Products.Include(p => p.DonViTinh).FirstOrDefaultAsync(p => p.Id == productId && !p.DeleteFlag, cancellationToken);
        }

        public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductName.ToLower() == name.ToLower() && !p.DeleteFlag, cancellationToken);   
        }

        public async Task<Product[]> GetListAysnc(string? keyword, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _dbContext.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                   x.ProductName.ToLower().Contains(keyword.ToLower()));
            }

            query = query
                .Where(x => !x.DeleteFlag)
                                .Include(p => p.DonViTinh);

            return await query.ToArrayAsync(cancellationToken);
        }

        public async Task<Category[]> GetListCategoryAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Categories.AsNoTracking().ToArrayAsync(cancellationToken);
        }

        public Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    };
}
