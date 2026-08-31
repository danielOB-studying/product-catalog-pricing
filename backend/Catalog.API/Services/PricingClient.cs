using System.Net.Http.Json;

namespace CatalogApi.Services
{
    /// <summary>Cliente HTTP hacia Pricing.API. Tolerante a fallos: si el cálculo no responde, el listado no se interrumpe.</summary>
    public sealed class PricingClient : IPricingClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<PricingClient> _logger;

        public PricingClient(HttpClient http, ILogger<PricingClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<PricingCalculationResult?> CalculateAsync(string categoryName, decimal basePrice, int quantity, DateTime date)
        {
            var payload = new
            {
                categoryName,
                basePrice,
                quantity,
                date,
            };

            try
            {
                var response = await _http.PostAsJsonAsync("/api/pricing/calculate", payload);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PricingCalculationResult>();
                }

                _logger.LogWarning("Pricing.API respondió con status {(int)} en /api/pricing/calculate", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo contactar a Pricing.API");
            }

            return null;
        }
    }
}