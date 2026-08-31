using CatalogApi.Common;
using CatalogApi.DTOs;
using CatalogApi.Models;
using CatalogApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers
{
    /// <summary>Gestión de categorías de producto.</summary>
    [Route("api/categories")]
    [ApiController]
    [Authorize(Roles = UserRoles.AdminOrViewer)]
    public sealed class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoryDto>> GetAll()
        {
            var categories = _unitOfWork.Categories().Query().OrderBy(c => c.Name).ToList();
            return Ok(categories.Select(ToDto));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult<CategoryDto> Create([FromBody] CreateCategoryDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "El nombre de la categoría es obligatorio" });
            }

            var category = new Category { Name = request.Name.Trim(), Description = request.Description };
            _unitOfWork.Categories().Add(category);
            _unitOfWork.Save();

            return CreatedAtAction(nameof(GetAll), new { id = category.Id }, ToDto(category));
        }

        private static CategoryDto ToDto(Category category) => new()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
        };
    }
}