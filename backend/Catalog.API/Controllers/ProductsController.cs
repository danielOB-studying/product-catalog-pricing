using CatalogApi.DTOs;
using CatalogApi.Models;
using CatalogApi.Repositories;
using CatalogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Controllers
{
    /// <summary>Gestión de productos. El listado enriquece cada producto con el precio final calculado por Pricing.API.</summary>
    [Route("api/products")]
    [ApiController]
    [Authorize(Roles = "Admin,Viewer")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPricingClient _pricingClient;

        public ProductsController(IUnitOfWork unitOfWork, IPricingClient pricingClient)
        {
            _unitOfWork = unitOfWork;
            _pricingClient = pricingClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductWithPriceDto>>> GetAll()
        {
            var products = await _unitOfWork
                .Products()
                .Query()
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var now = DateTime.Now;
            var result = new List<ProductWithPriceDto>();

            foreach (var product in products)
            {
                var pricing = await _pricingClient.CalculateAsync(product.Category.Name, product.BasePrice, product.Stock, now);
                var dto = ToDto(product);
                result.Add(new ProductWithPriceDto
                {
                    Id = dto.Id,
                    Sku = dto.Sku,
                    Name = dto.Name,
                    Description = dto.Description,
                    BasePrice = dto.BasePrice,
                    Stock = dto.Stock,
                    CategoryId = dto.CategoryId,
                    CategoryName = dto.CategoryName,
                    FinalPrice = pricing?.FinalPrice ?? dto.BasePrice,
                    AppliedDiscount = pricing?.AppliedDiscount,
                });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<ProductDto> Create([FromBody] CreateProductDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "El SKU y el nombre son obligatorios" });
            }

            var category = _unitOfWork.Categories().Find(request.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "Categoría no encontrada" });
            }

            var product = new Product
            {
                Sku = request.Sku.Trim(),
                Name = request.Name.Trim(),
                Description = request.Description,
                BasePrice = request.BasePrice,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
            };
            _unitOfWork.Products().Add(product);
            _unitOfWork.Save();

            return StatusCode(201, ToDto(product));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<ProductDto> Update(int id, [FromBody] UpdateProductDto request)
        {
            var product = _unitOfWork.Products().Find(id);
            if (product == null)
            {
                return NotFound(new { message = $"Producto {id} no encontrado" });
            }

            if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "El SKU y el nombre son obligatorios" });
            }

            var category = _unitOfWork.Categories().Find(request.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "Categoría no encontrada" });
            }

            product.Sku = request.Sku.Trim();
            product.Name = request.Name.Trim();
            product.Description = request.Description;
            product.BasePrice = request.BasePrice;
            product.Stock = request.Stock;
            product.CategoryId = request.CategoryId;
            _unitOfWork.Save();

            return Ok(ToDto(product));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.Products().Find(id);
            if (product == null)
            {
                return NotFound(new { message = $"Producto {id} no encontrado" });
            }

            _unitOfWork.Products().Remove(product);
            _unitOfWork.Save();

            return NoContent();
        }

        private static ProductDto ToDto(Product product) => new()
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            BasePrice = product.BasePrice,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
        };
    }
}