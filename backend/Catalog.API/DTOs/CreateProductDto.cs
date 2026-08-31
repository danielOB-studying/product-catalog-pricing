namespace CatalogApi.DTOs
{
    /// <summary>Cuerpo de entrada para crear un producto (rol Admin).</summary>
    public sealed class CreateProductDto
    {
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }
}