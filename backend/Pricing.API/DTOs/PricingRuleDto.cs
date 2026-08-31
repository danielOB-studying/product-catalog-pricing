namespace PricingApi.DTOs
{
    /// <summary>Representación pública de una regla de descuento.</summary>
    public sealed class PricingRuleDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MinQuantity { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}