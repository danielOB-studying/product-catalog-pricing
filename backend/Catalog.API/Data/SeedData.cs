using System.Security.Cryptography;
using System.Text;
using CatalogApi.Models;
using CatalogApi.Services;

namespace CatalogApi.Data
{
    /// <summary>Semilla inicial de usuarios (con hash PBKDF2) y catálogo demo.</summary>
    public sealed class SeedData
    {
        private SeedData()
        {
        }

        /// <summary>Inserta los registros semilla si la base está vacía. `seedDemoData` controla el catálogo demo.</summary>
        public static void Run(Data.AppDbContext context, IPasswordHasher passwordHasher, bool seedDemoData)
        {
            if (!context.ApplicationUsers.Any())
            {
                context.ApplicationUsers.Add(new ApplicationUser
                {
                    Username = "admin",
                    PasswordHash = passwordHasher.Hash("admin123"),
                    Role = "Admin",
                });
                context.ApplicationUsers.Add(new ApplicationUser
                {
                    Username = "viewer",
                    PasswordHash = passwordHasher.Hash("viewer123"),
                    Role = "Viewer",
                });
            }

            if (seedDemoData && !context.Categories.Any())
            {
                var electronica = new Category { Name = "Electrónica", Description = "Dispositivos y accesorios electrónicos" };
                var hogar = new Category { Name = "Hogar", Description = "Artículos para el hogar" };
                var oficina = new Category { Name = "Oficina", Description = "Suministros y mobiliario de oficina" };

                context.Categories.Add(electronica);
                context.Categories.Add(hogar);
                context.Categories.Add(oficina);
                context.SaveChanges();

                if (!context.Products.Any())
                {
                    var productos = new List<Product>
                    {
                        new Product { Sku = "ELEC-001", Name = "Audífonos Bluetooth", Description = "Inalámbricos con cancelación de ruido", BasePrice = 89.99m, Stock = 40, CategoryId = electronica.Id },
                        new Product { Sku = "ELEC-002", Name = "Smartwatch Series X", Description = "Reloj inteligente con GPS y monitor cardiaco", BasePrice = 199.00m, Stock = 15, CategoryId = electronica.Id },
                        new Product { Sku = "HOGAR-001", Name = "Licuadora Pro", Description = "Motor de 1000W con 4 velocidades", BasePrice = 59.50m, Stock = 25, CategoryId = hogar.Id },
                        new Product { Sku = "HOGAR-002", Name = "Set de Ollas Antiadherentes", Description = "Juego de 5 piezas de acero inoxidable", BasePrice = 129.99m, Stock = 8, CategoryId = hogar.Id },
                        new Product { Sku = "OFIC-001", Name = "Silla Ergonómica", Description = "Con soporte lumbar y apoyabrazos ajustables", BasePrice = 249.00m, Stock = 4, CategoryId = oficina.Id },
                        new Product { Sku = "OFIC-002", Name = "Resma de Papel A4", Description = "Papel de 90g para impresora (500 hojas)", BasePrice = 4.75m, Stock = 300, CategoryId = oficina.Id },
                    };
                    context.Products.AddRange(productos);
                }
            }

            context.SaveChanges();
        }
    }
}