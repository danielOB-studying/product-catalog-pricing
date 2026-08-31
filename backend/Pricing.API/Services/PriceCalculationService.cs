using PricingApi.Models;
using PricingApi.Repositories;

namespace PricingApi.Services
{
    /// <summary>
    /// Aplica la regla activa más relevante sobre el precio base. Criterio:
    /// 1. Se consideran solo las reglas activas que aplican al input (por categoría, fecha y/o volumen).
    /// 2. Se elige la de mayor descuento; en caso de empate, el tipo con prioridad más alta
    ///    (ByVolume &gt; ByDateRange &gt; ByCategory).
    /// 3. finalPrice = basePrice - (basePrice * porcentaje / 100), redondeado a 2 decimales.
    /// </summary>
    public sealed class PriceCalculationService : IPriceCalculationService
    {
        private static readonly Dictionary<DiscountType, int> Priority = new()
        {
            { DiscountType.ByVolume, 0 },
            { DiscountType.ByDateRange, 1 },
            { DiscountType.ByCategory, 2 },
        };

        private readonly IUnitOfWork _unitOfWork;

        public PriceCalculationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PriceCalculationResult Calculate(PriceCalculationInput input)
        {
            var applicable = _unitOfWork
                .PricingRules()
                .Query()
                .Where(rule => rule.IsActive)
                .AsEnumerable()
                .Where(rule => IsApplicable(rule, input))
                .ToList();

            // Mayor descuento; empate -> menor índice de prioridad.
            var best = applicable
                .OrderByDescending(rule => rule.DiscountPercentage)
                .ThenBy(rule => Priority[rule.Type])
                .FirstOrDefault();

            if (best == null)
            {
                return new PriceCalculationResult { FinalPrice = input.BasePrice, AppliedDiscount = null };
            }

            var discountAmount = decimal.Round(input.BasePrice * best.DiscountPercentage / 100m, 2);
            var finalPrice = decimal.Round(input.BasePrice - discountAmount, 2);

            return new PriceCalculationResult { FinalPrice = finalPrice, AppliedDiscount = best.DiscountPercentage };
        }

        private static bool IsApplicable(PricingRule rule, PriceCalculationInput input)
        {
            return rule.Type switch
            {
                DiscountType.ByCategory =>
                    rule.CategoryName != null && rule.CategoryName == input.CategoryName,

                DiscountType.ByDateRange =>
                    rule.StartDate != null && rule.EndDate != null
                    && input.Date >= rule.StartDate && input.Date <= rule.EndDate,

                DiscountType.ByVolume =>
                    rule.MinQuantity != null && input.Quantity >= rule.MinQuantity,

                _ => false,
            };
        }
    }
}