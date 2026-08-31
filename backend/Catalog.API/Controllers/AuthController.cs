using CatalogApi.Data;
using CatalogApi.DTOs;
using CatalogApi.Models;
using CatalogApi.Repositories;
using CatalogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers
{
    /// <summary>Autenticación: valida credenciales y emite el JWT.</summary>
    [Route("api/auth")]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtTokenService _jwt;

        public AuthController(IUnitOfWork unitOfWork, JwtTokenService jwt)
        {
            _unitOfWork = unitOfWork;
            _jwt = jwt;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto request)
        {
            var user = _unitOfWork
                .ApplicationUsers()
                .Query()
                .FirstOrDefault(u => u.Username == request.Username);

            if (user == null || !MatchesPassword(user, request.Password))
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(new LoginResponseDto
            {
                Token = _jwt.Generate(user),
                Username = user.Username,
                Role = user.Role,
            });
        }

        private static bool MatchesPassword(ApplicationUser user, string password)
        {
            return SeedData.HashPassword(password) == user.PasswordHash;
        }
    }
}