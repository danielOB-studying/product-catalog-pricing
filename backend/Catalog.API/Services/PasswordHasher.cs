using System.Security.Cryptography;
using System.Text;

namespace CatalogApi.Services
{
    /// <summary>Implementación PBKDF2-SHA256 del hashing de contraseñas (sin dependencias externas).</summary>
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const string Salt = "catalog-salt";
        private const int Iterations = 100000;
        private const int OutputLengthBytes = 32;

        public string Hash(string password)
        {
            var saltBytes = Encoding.UTF8.GetBytes(Salt);
            var derived = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, iterations: Iterations, HashAlgorithmName.SHA256, outputLength: OutputLengthBytes);
            return Convert.ToHexString(derived);
        }

        public bool Verify(string password, string passwordHash)
        {
            return Hash(password) == passwordHash;
        }
    }
}