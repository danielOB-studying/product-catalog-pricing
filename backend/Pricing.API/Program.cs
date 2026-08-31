using System.Text.Json.Serialization;
using PricingApi.Data;
using PricingApi.Repositories;
using PricingApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers, repositorios (Repository + Unit of Work) sobre EF Core + SQLite.
// Los enums (DiscountType) se aceptan/serializan como strings ("ByCategory") para legibilidad.
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
var pricingConn = builder.Configuration.GetConnectionString("PricingDb") ?? "Data Source=pricing.db";
builder.Services.AddDbContext<PricingDbContext>(options => options.UseSqlite(pricingConn));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Motor de cálculo de precios (lógica de negocio del dominio de precios).
builder.Services.AddScoped<IPriceCalculationService, PriceCalculationService>();

// Swagger / OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pricing.API", Version = "v1", Description = "Motor de precios: reglas de descuento y cálculo de precio final." });
});

// CORS: orígenes permitidos desde configuración (ver appsettings.json → Cors:AllowedOrigins).
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] { "http://localhost:4200" };
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins(allowedOrigins)));

var app = builder.Build();

// Aplica migraciones automáticas (demo) y siembra reglas de precio demo.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PricingDbContext>();
    db.Database.Migrate();
    SeedData.Run(db, seedDemoData: true);
}

app.UseCors("Frontend");

app.UseSwagger();
app.UseSwaggerUI(ui => ui.SwaggerEndpoint("/swagger/v1/swagger.json", "Pricing.API v1"));

app.UseAuthorization();
app.MapControllers();

app.Run();