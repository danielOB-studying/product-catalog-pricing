using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CatalogApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace CatalogApi.Services
{
    /// <summary>Genera el token JWT para autenticación del backoffice.</summary>
    public sealed class JwtTokenService
    {
        private const string Issuer = "Catalog.API";
        private const string Audience = "catalog-backoffice";

        private readonly SymmetricSecurityKey _signingKey;
        private readonly int _expirationMinutes;

        public JwtTokenService(IConfiguration config)
        {
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
            _expirationMinutes = config.GetValue("JWT:ExpirationMinutes", 480);
        }

        public string Generate(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
                signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}