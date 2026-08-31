namespace CatalogApi.DTOs
{
    /// <summary>Representación pública de un producto (sin relación a categoría ni precio calculado).</summary>
    public class ProductDto
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}