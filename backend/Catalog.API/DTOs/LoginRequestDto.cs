namespace CatalogApi.DTOs
{
    /// <summary>Cuerpo de entrada para el login.</summary>
    public sealed class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}