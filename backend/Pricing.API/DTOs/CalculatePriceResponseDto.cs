namespace PricingApi.DTOs
{
    /// <summary>Respuesta del endpoint de cálculo: precio final y descuento aplicado, en porcentaje.</summary>
    public sealed class CalculatePriceResponseDto
    {
        public decimal FinalPrice { get; set; }
        public decimal? AppliedDiscount { get; set; }
    }
}