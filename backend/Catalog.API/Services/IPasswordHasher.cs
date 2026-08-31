namespace CatalogApi.Services
{
    /// <summary>Contrato de hashing/verificación de contraseñas (PBKDF2).</summary>
    public interface IPasswordHasher
    {
        /// <summary>Deriva el hash PBKDF2-SHA256 (32 bytes, 100000 iteraciones) de una contraseña en claro.</summary>
        string Hash(string password);

        /// <summary>Indica si la contraseña en claro corresponde al hash almacenado.</summary>
        bool Verify(string password, string passwordHash);
    }
}