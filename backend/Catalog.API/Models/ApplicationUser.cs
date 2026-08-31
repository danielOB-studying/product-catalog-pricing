namespace CatalogApi.Models
{
    /// <summary>Usuario del backoffice con rol para autorización por endpoint.</summary>
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Admin" | "Viewer"
    }
}