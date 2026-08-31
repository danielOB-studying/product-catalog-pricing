namespace CatalogApi.DTOs
{
    /// <summary>Representación pública de una categoría (no expone las relaciones del modelo).</summary>
    public sealed class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}