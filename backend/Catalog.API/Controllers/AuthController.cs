using CatalogApi.Common;
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
        private readonly IJwtTokenService _jwt;
        private readonly IPasswordHasher _passwordHasher;

        public AuthController(IUnitOfWork unitOfWork, IJwtTokenService jwt, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _jwt = jwt;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto request)
        {
            var user = _unitOfWork
                .ApplicationUsers()
                .Query()
                .FirstOrDefault(u => u.Username == request.Username);

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
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
    }
}