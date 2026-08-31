using PricingApi.Data;
using PricingApi.Repositories;
using PricingApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers, repositorios (Repository + Unit of Work) sobre EF Core + SQLite.
builder.Services.AddControllers();
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

// CORS para permitir, si se decide, el consumo directo desde el frontend Angular.
builder.Services.AddCors();

var app = builder.Build();

// Aplica migraciones automáticas (demo) y siembra datos de desarrollo.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PricingDbContext>();
    db.Database.Migrate();
    SeedData.Run(db, true);
}

app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
       .WithOrigins("http://localhost:4200"));

app.UseSwagger();
app.UseSwaggerUI(ui => ui.SwaggerEndpoint("/swagger/v1/swagger.json", "Pricing.API v1"));

app.UseAuthorization();
app.MapControllers();

app.Run();