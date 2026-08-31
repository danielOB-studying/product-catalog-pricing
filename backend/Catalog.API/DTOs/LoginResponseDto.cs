namespace CatalogApi.DTOs
{
    /// <summary>Respuesta del login con el token JWT de acceso.</summary>
    public sealed class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}