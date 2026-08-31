using PricingApi.Models;

namespace PricingApi.Data
{
    /// <summary>Semilla inicial de reglas de descuento para la demo.</summary>
    public sealed class SeedData
    {
        private SeedData()
        {
        }

        /// <summary>Inserta las reglas semilla si la base está vacía y es entorno de desarrollo.</summary>
        public static void Run(Data.PricingDbContext context, bool isDevelopment)
        {
            if (!isDevelopment || context.PricingRules.Any())
            {
                return;
            }

            var today = DateTime.UtcNow.Date;
            var todayPlus10 = today.AddDays(10);

            context.PricingRules.AddRange(new List<PricingRule>
            {
                new PricingRule { Type = DiscountType.ByCategory, CategoryName = "Electrónica", DiscountPercentage = 5m, IsActive = true },
                new PricingRule { Type = DiscountType.ByCategory, CategoryName = "Hogar", DiscountPercentage = 10m, IsActive = true },
                new PricingRule { Type = DiscountType.ByDateRange, StartDate = today, EndDate = todayPlus10, DiscountPercentage = 15m, IsActive = true },
                new PricingRule { Type = DiscountType.ByVolume, MinQuantity = 50, DiscountPercentage = 20m, IsActive = true },
            });

            context.SaveChanges();
        }
    }
}