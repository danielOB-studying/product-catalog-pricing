namespace PricingApi.Models
{
    /// <summary>Regla de descuento del dominio de precios. No conoce nada del catálogo ni de sus Ids.</summary>
    public class PricingRule
    {
        public int Id { get; set; }
        public DiscountType Type { get; set; }
        public string? CategoryName { get; set; }     // usado si Type == ByCategory
        public DateTime? StartDate { get; set; }       // usado si Type == ByDateRange
        public DateTime? EndDate { get; set; }
        public int? MinQuantity { get; set; }          // usado si Type == ByVolume
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}