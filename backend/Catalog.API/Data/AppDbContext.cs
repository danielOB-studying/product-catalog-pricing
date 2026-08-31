using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Data
{
    /// <summary>Contexto de EF Core del servicio de catálogo. Base de datos independiente.</summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(category => category.Name).IsRequired();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(product => product.Sku).IsRequired();
                entity.Property(product => product.Name).IsRequired();
                entity.Property(product => product.BasePrice).IsRequired();
                entity.Property(product => product.Stock).IsRequired();
                entity.Property(product => product.CategoryId).IsRequired();
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(user => user.Username).IsRequired();
                entity.Property(user => user.PasswordHash).IsRequired();
                entity.Property(user => user.Role).IsRequired();
            });
        }
    }
}