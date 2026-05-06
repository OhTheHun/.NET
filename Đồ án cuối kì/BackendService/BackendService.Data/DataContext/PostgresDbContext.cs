using BackendService.Model;
using BackendService.Model.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace BackendService.Data.DataContext
{
    public partial class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }
        public virtual DbSet<Import> Imports { get; set; }
        public virtual DbSet<ImportDetail> ImportDetails { get; set; }
        public virtual DbSet<DonViTinh> DonViTinhs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json")
                       .Build();

                var connectionString = configuration.GetConnectionString("Postgres");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<EmployeeProfile>().ToTable("EmployeeProfiles");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Supplier>().ToTable("Suppliers");
            modelBuilder.Entity<Inventory>().ToTable("Inventories");
            modelBuilder.Entity<Promotion>().ToTable("Promotions");
            modelBuilder.Entity<Invoice>().ToTable("Invoices");
            modelBuilder.Entity<InvoiceItem>().ToTable("InvoiceItems");
            modelBuilder.Entity<Import>().ToTable("Imports");
            modelBuilder.Entity<ImportDetail>().ToTable("ImportDetails");
            modelBuilder.Entity<DonViTinh>().ToTable("DonViTinhs");
        }

    }

}


