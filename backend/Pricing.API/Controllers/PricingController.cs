using Microsoft.AspNetCore.Mvc;
using PricingApi.DTOs;
using PricingApi.Services;

namespace PricingApi.Controllers
{
    /// <summary>Motor de cálculo de precio final. Recibe parámetros del producto y devuelve precio final + descuento.</summary>
    [Route("api/pricing")]
    [ApiController]
    public sealed class PricingController : ControllerBase
    {
        private readonly IPriceCalculationService _priceCalculationService;

        public PricingController(IPriceCalculationService priceCalculationService)
        {
            _priceCalculationService = priceCalculationService;
        }

        [HttpPost("calculate")]
        public ActionResult<CalculatePriceResponseDto> Calculate([FromBody] CalculatePriceRequestDto request)
        {
            var result = _priceCalculationService.Calculate(new PriceCalculationInput(
                request.CategoryName ?? string.Empty,
                request.BasePrice,
                request.Quantity ?? 0,
                request.Date ?? DateTime.Now));

            return Ok(new CalculatePriceResponseDto
            {
                FinalPrice = result.FinalPrice,
                AppliedDiscount = result.AppliedDiscount,
            });
        }
    }
}