using PricingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace PricingApi.Data
{
    /// <summary>Contexto de EF Core del servicio de precios. Base de datos independiente.</summary>
    public class PricingDbContext : DbContext
    {
        public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options)
        {
        }

        public DbSet<PricingRule> PricingRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PricingRule>(entity =>
            {
                entity.Property(rule => rule.Type).IsRequired();
                entity.Property(rule => rule.DiscountPercentage).IsRequired();
                entity.Property(rule => rule.IsActive).IsRequired();
            });
        }
    }
}