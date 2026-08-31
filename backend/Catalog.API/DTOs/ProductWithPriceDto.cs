namespace CatalogApi.DTOs
{
    /// <summary>Respuesta del listado de productos enriquecida con el precio final calculado por Pricing.API.</summary>
    public sealed class ProductWithPriceDto : ProductDto
    {
        public decimal FinalPrice { get; set; }
        public decimal? AppliedDiscount { get; set; }
    }
}