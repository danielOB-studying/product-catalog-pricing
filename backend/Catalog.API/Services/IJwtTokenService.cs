namespace CatalogApi.Services
{
    /// <summary>Contrato de emisión de tokens JWT para el backoffice.</summary>
    public interface IJwtTokenService
    {
        /// <summary>Genera un JWT firmado (HS256) con los claims del usuario.</summary>
        string Generate(Models.ApplicationUser user);
    }
}