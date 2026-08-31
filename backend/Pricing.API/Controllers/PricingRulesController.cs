using Microsoft.AspNetCore.Mvc;
using PricingApi.DTOs;
using PricingApi.Models;
using PricingApi.Repositories;

namespace PricingApi.Controllers
{
    /// <summary>CRUD de reglas de descuento del dominio de precios. No conoce nada del catálogo.</summary>
    [Route("api/pricing-rules")]
    [ApiController]
    public sealed class PricingRulesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PricingRulesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PricingRuleDto>> GetAll()
        {
            var rules = _unitOfWork.PricingRules().Query().OrderBy(r => r.Id).ToList();
            return Ok(rules.Select(ToDto));
        }

        [HttpPost]
        public ActionResult<PricingRuleDto> Create([FromBody] CreatePricingRuleDto request)
        {
            if (!IsValid(request.Type, request.DiscountPercentage, request.CategoryName, request.StartDate, request.EndDate, request.MinQuantity, out var message))
            {
                return BadRequest(new { message });
            }

            var rule = new PricingRule
            {
                Type = request.Type,
                CategoryName = request.CategoryName,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MinQuantity = request.MinQuantity,
                DiscountPercentage = request.DiscountPercentage,
                IsActive = request.IsActive,
            };
            _unitOfWork.PricingRules().Add(rule);
            _unitOfWork.Save();

            return StatusCode(201, ToDto(rule));
        }

        [HttpPut("{id:int}")]
        public ActionResult<PricingRuleDto> Update(int id, [FromBody] UpdatePricingRuleDto request)
        {
            var rule = _unitOfWork.PricingRules().Find(id);
            if (rule == null)
            {
                return NotFound(new { message = $"Regla {id} no encontrada" });
            }

            if (!IsValid(request.Type, request.DiscountPercentage, request.CategoryName, request.StartDate, request.EndDate, request.MinQuantity, out var message))
            {
                return BadRequest(new { message });
            }

            rule.Type = request.Type;
            rule.CategoryName = request.CategoryName;
            rule.StartDate = request.StartDate;
            rule.EndDate = request.EndDate;
            rule.MinQuantity = request.MinQuantity;
            rule.DiscountPercentage = request.DiscountPercentage;
            rule.IsActive = request.IsActive;
            _unitOfWork.Save();

            return Ok(ToDto(rule));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var rule = _unitOfWork.PricingRules().Find(id);
            if (rule == null)
            {
                return NotFound(new { message = $"Regla {id} no encontrada" });
            }

            _unitOfWork.PricingRules().Remove(rule);
            _unitOfWork.Save();

            return NoContent();
        }

        private static bool IsValid(
            DiscountType type,
            decimal discountPercentage,
            string? categoryName,
            DateTime? startDate,
            DateTime? endDate,
            int? minQuantity,
            out string message)
        {
            if (discountPercentage <= 0 || discountPercentage > 100)
            {
                message = "El descuento debe estar entre 0 y 100";
                return false;
            }

            if (type == DiscountType.ByCategory && string.IsNullOrWhiteSpace(categoryName))
            {
                message = "Las reglas por categoría requieren CategoryName";
                return false;
            }

            if (type == DiscountType.ByDateRange && (startDate == null || endDate == null || endDate < startDate))
            {
                message = "Las reglas por fecha requieren StartDate y EndDate válidos";
                return false;
            }

            if (type == DiscountType.ByVolume && (minQuantity == null || minQuantity <= 0))
            {
                message = "Las reglas por volumen requieren MinQuantity mayor a 0";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private static PricingRuleDto ToDto(PricingRule rule) => new()
        {
            Id = rule.Id,
            Type = rule.Type.ToString(),
            CategoryName = rule.CategoryName,
            StartDate = rule.StartDate,
            EndDate = rule.EndDate,
            MinQuantity = rule.MinQuantity,
            DiscountPercentage = rule.DiscountPercentage,
            IsActive = rule.IsActive,
        };
    }
}