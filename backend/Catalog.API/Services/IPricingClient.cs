namespace CatalogApi.Services
{
    /// <summary>Resultado del cálculo de precio devuelto por Pricing.API.</summary>
    public sealed class PricingCalculationResult
    {
        public decimal FinalPrice { get; set; }
        public decimal? AppliedDiscount { get; set; }
    }

    /// <summary>Contrato del cliente HTTP hacia Pricing.API.</summary>
    public interface IPricingClient
    {
        /// <summary>Invoca POST /api/pricing/calculate en Pricing.API y devuelve null si no aplica descuento o si falla.</summary>
        Task<PricingCalculationResult?> CalculateAsync(string categoryName, decimal basePrice, int quantity, DateTime date);
    }
}