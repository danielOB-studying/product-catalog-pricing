using System.Text;
using CatalogApi.Data;
using CatalogApi.Repositories;
using CatalogApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers, repositorios (Repository + Unit of Work) sobre EF Core + SQLite.
builder.Services.AddControllers();
var catalogConn = builder.Configuration.GetConnectionString("CatalogDb") ?? "Data Source=catalog.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(catalogConn));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Cliente HTTP hacia Pricing.API (REST síncrono interno).
var pricingBaseUrl = builder.Configuration["PricingApi:BaseUrl"] ?? "http://localhost:5002";
builder.Services.AddHttpClient<IPricingClient, PricingClient>(client => client.BaseAddress = new Uri(pricingBaseUrl));

// Autenticación / autorización con JWT emitido por este servicio.
builder.Services.AddSingleton<JwtTokenService>();
var jwtKey = builder.Configuration["JWT:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "Catalog.API",
            ValidateAudience = true,
            ValidAudience = "catalog-backoffice",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();

// Swagger / OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1", Description = "Backoffice de catálogo: producto, categoría y autenticación." });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Copie el token JWT obtenido en /api/auth/login.",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            },
            Array.Empty<string>()
        },
    });
});

// CORS para permitir el consumo desde el frontend Angular.
builder.Services.AddCors();

var app = builder.Build();

// Aplica migraciones automáticas (demo) y siembra datos de desarrollo.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    SeedData.Run(db, true);
}

app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:4200"));

app.UseSwagger();
app.UseSwaggerUI(ui => ui.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();