using BackendService.Data.DataContext;
using BackendService.Data.Interface;
using BackendService.Model;
using BackendService.Model.Common;
using BackendService.Model.Enums;
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
                .Where(x => x.Status == ProductEnum.Active )
                                .Include(p => p.DonViTinh);

            return await query.ToArrayAsync(cancellationToken);
        }

        public async Task<Product[]> GetAdminListAsync(string? keyword, Guid? categoryId, int? status, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.DonViTinh)
                .Include(p => p.Supplier)
                .Where(x => !x.DeleteFlag);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                   x.ProductName.ToLower().Contains(keyword.ToLower()) ||
                   x.SKU.ToLower().Contains(keyword.ToLower()));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => (int)x.Status == status.Value);
            }

            return await query.OrderByDescending(x => x.CreatedTime).ToArrayAsync(cancellationToken);
        }

        public async Task<Category[]> GetListCategoryAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Categories.Where(c => !c.DeleteFlag).AsNoTracking().ToArrayAsync(cancellationToken);
        }

        public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task SoftDeleteAsync(Guid productId, string actor, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(new object[] { productId }, cancellationToken);
            if (product != null)
            {
                product.DeleteFlag = true;
                product.UpdatedTime = DateTime.UtcNow;
                product.UpdatedBy = actor;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    };
}
