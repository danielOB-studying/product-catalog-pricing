using PricingApi.Models;

namespace PricingApi.Services
{
    /// <summary>Parámetros de entrada para calcular el precio final de un producto.</summary>
    public sealed class PriceCalculationInput
    {
        public PriceCalculationInput(string categoryName, decimal basePrice, int quantity, DateTime date)
        {
            CategoryName = categoryName;
            BasePrice = basePrice;
            Quantity = quantity;
            Date = date;
        }

        public string CategoryName { get; }
        public decimal BasePrice { get; }
        public int Quantity { get; }
        public DateTime Date { get; }
    }

    /// <summary>Resultado del cálculo de precio.</summary>
    public sealed class PriceCalculationResult
    {
        public decimal FinalPrice { get; set; }
        public decimal? AppliedDiscount { get; set; }
    }

    /// <summary>Contrato del motor de cálculo de precios y descuentos.</summary>
    public interface IPriceCalculationService
    {
        PriceCalculationResult Calculate(PriceCalculationInput input);
    }
}