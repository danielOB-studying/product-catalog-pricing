using System.Collections.Generic;

namespace CatalogApi.Models
{
    /// <summary>Sustantivo de dominio que agrupa productos.</summary>
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}