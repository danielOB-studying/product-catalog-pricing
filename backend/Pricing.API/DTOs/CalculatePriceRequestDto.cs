namespace PricingApi.DTOs
{
    /// <summary>Parámetros que envía Catalog.API (o el frontend) al endpoint de cálculo de precio.</summary>
    public sealed class CalculatePriceRequestDto
    {
        public int? Quantity { get; set; }
        public DateTime? Date { get; set; }
        public decimal BasePrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}