using PricingApi.Data;
using PricingApi.Models;

namespace PricingApi.Repositories
{
    /// <summary>Agrupa los repositorios del dominio de precios y gestiona la persistencia en conjunto.</summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PricingDbContext _context;
        private readonly EFRepository<PricingRule> _pricingRules;

        public UnitOfWork(PricingDbContext context)
        {
            _context = context;
            _pricingRules = new EFRepository<PricingRule>(context.PricingRules);
        }

        public IRepository<PricingRule> PricingRules() => _pricingRules;

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}