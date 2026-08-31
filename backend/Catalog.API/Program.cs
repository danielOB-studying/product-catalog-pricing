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

// Hashing de contraseñas (PBKDF2) para autenticación y seed.
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// Cliente HTTP hacia Pricing.API (REST síncrono interno).
var pricingBaseUrl = builder.Configuration["PricingApi:BaseUrl"] ?? "http://localhost:5002";
builder.Services.AddHttpClient<IPricingClient, PricingClient>(client => client.BaseAddress = new Uri(pricingBaseUrl));

// Autenticación / autorización con JWT emitido por este servicio.
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
var jwtKey = builder.Configuration["JWT:Key"]!;
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "Catalog.API";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "catalog-backoffice";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
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

// CORS: orígenes permitidos desde configuración (ver appsettings.json → Cors:AllowedOrigins).
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] { "http://localhost:4200" };
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins(allowedOrigins)));

var app = builder.Build();

// Aplica migraciones automáticas (demo) y siembra datos demo (usuarios + catálogo de ejemplo).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    db.Database.Migrate();
    SeedData.Run(db, passwordHasher, seedDemoData: true);
}

app.UseCors("Frontend");

app.UseSwagger();
app.UseSwaggerUI(ui => ui.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();