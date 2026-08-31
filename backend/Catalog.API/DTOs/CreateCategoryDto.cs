namespace CatalogApi.DTOs
{
    /// <summary>Cuerpo de entrada para crear una categoría (rol Admin).</summary>
    public sealed class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}