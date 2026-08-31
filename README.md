# Backoffice de Catálogo de Productos con Motor de Precios

Backoffice funcional que separa el **catálogo de productos** de un **motor de reglas de precios**, como servicios independientes con bases de datos propias (SQLite). El catálogo orquesta la consulta al motor de precios para mostrar el *precio final* de cada producto.

## Arquitectura

```
┌─────────────────────┐
│   Angular SPA        │
│  (Admin + Catálogo)  │
└──────────┬───────────┘
           │ HTTPS / REST + JWT
           ▼
┌─────────────────────┐        REST interno       ┌─────────────────────┐
│   Catalog.API        │ ─────────────────────────▶│   Pricing.API      |
|                       │                           |  Microservicio      |
│   (.NET 8/9)          │◀─────────────────────────│   (.NET 8/9)          │
│   - Productos          │      precio calculado     │   - Reglas de precio  │
│   - Categorías         │                           │   - Cálculo de desc.  │
│   - Auth/JWT            │                           │                       │
└──────────┬───────────┘                           └──────────┬───────────┘
           │ EF Core                                            │ EF Core
           ▼                                                    ▼
   catalog.db (SQLite)                                  pricing.db (SQLite)
```

| Servicio | Responsabilidad | Puerto |
|---|---|---|
| **Catalog.API** (`backend/Catalog.API`) | Dominio de productos/categorías, autenticación JWT y orquestación del precio final | 5001 |
| **Pricing.API** (`backend/Pricing.API`) | Microservicio - Dominio de reglas de precio y cálculo de descuentos. No conoce el catálogo (se referencia por nombre de categoría, no por Id) | 5002 |

## Decisiones de diseño

- **Repository + Unit of Work** sobre EF Core: desacopla la lógica de negocio del proveedor de base de datos (migrar de SQLite a SQL Server requiere tocar solo la infraestructura).
- **DTOs explícitos**: las entidades de EF Core nunca salen en los contratos de API.
- **Comunicación REST síncrona** entre servicios: el cálculo de precio en pantalla es una lectura que necesita respuesta inmediata; la mensajería asíncrona (RabbitMQ/Kafka) quedó como roadmap.
- **Tolerancia a fallos**: si Pricing.API no responde, el listado de productos devuelve el precio base (sin precio enriquecido) en lugar de fallar completo.
- **Criterio de descuento**: se aplican las reglas activas y aplicables (categoría, rango de fechas, volumen); gana la de **mayor porcentaje** de descuento. En empate, prioridad `ByVolume > ByDateRange > ByCategory` para determinismo.
- **Seguridad**: JWT firmado con clave simétrica (`appsettings.json` → `JWT:Key`), emitido por Catalog.API, con roles `Admin` (escritura) y `Viewer` (solo lectura).
- **Objetivo .NET**: el proyecto usa el SDK disponible en la máquina (verificado con `dotnet build`).

## Contratos de API

### Catalog.API (Swagger: `http://localhost:5001/swagger`)
| Método | Endpoint | Rol |
|---|---|---|
| POST | `/api/auth/login` | Público |
| GET | `/api/categories` | Viewer/Admin |
| POST | `/api/categories` | Admin |
| GET | `/api/products` (con precio final) | Viewer/Admin |
| POST / PUT / DELETE | `/api/products[...]` | Admin |

### Pricing.API (Swagger: `http://localhost:5002/swagger`)
| Método | Endpoint |
|---|---|
| GET / POST | `/api/pricing-rules` |
| PUT / DELETE | `/api/pricing-rules/{id}` |
| POST | `/api/pricing/calculate` → `{ categoryName, basePrice, quantity, date }` → `{ finalPrice, appliedDiscount }` |

## Ejecución local

Requisitos: .NET SDK 8+.

```powershell
# Terminal 1 — Pricing.API
dotnet run --project backend/Pricing.API --urls http://localhost:5002

# Terminal 2 — Catalog.API (debe iniciarse después de Pricing.API)
dotnet run --project backend/Catalog.API --urls http://localhost:5001
```

Al arrancar, cada servicio aplica sus migraciones y siembra datos demo automáticamente (SQLite: `catalog.db` y `pricing.db`).

### Usuarios semilla (demo)
| Usuario | Contraseña | Rol |
|---|---|---|
| `admin` | `admin123` | Admin |
| `viewer` | `viewer123` | Viewer |

> Solo para desarrollo local. En producción usar hash real + secretos gestionados.

### Prueba rápida
```powershell
$login = Invoke-RestMethod -Uri http://localhost:5001/api/auth/login -Method Post -ContentType 'application/json' -Body '{"username":"admin","password":"admin123"}'
$h = @{ Authorization = "Bearer $($login.token)" }
Invoke-RestMethod -Uri http://localhost:5001/api/products -Headers $h   # productos con precio final
Invoke-RestMethod -Uri http://localhost:5002/api/pricing-rules           # reglas semilla
```

## Datos semilla
- **Catálogo**: 3 categorías (Electrónica, Hogar, Oficina) y 6 productos.
- **Reglas de precio**: 4 reglas demo (por categoría, por rango de fechas y por volumen).

