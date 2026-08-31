using PricingApi.Models;

namespace PricingApi.DTOs
{
    /// <summary>Cuerpo de entrada para crear una regla de descuento.</summary>
    public sealed class CreatePricingRuleDto
    {
        public DiscountType Type { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MinQuantity { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}