using CatalogApi.Data;
using CatalogApi.Models;

namespace CatalogApi.Repositories
{
    /// <summary>Agrupa los repositorios del catálogo y gestiona la persistencia en conjunto.</summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly EFRepository<Category> _categories;
        private readonly EFRepository<Product> _products;
        private readonly EFRepository<ApplicationUser> _users;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _categories = new EFRepository<Category>(context.Categories);
            _products = new EFRepository<Product>(context.Products);
            _users = new EFRepository<ApplicationUser>(context.ApplicationUsers);
        }

        public IRepository<Category> Categories() => _categories;
        public IRepository<Product> Products() => _products;
        public IRepository<ApplicationUser> ApplicationUsers() => _users;

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}