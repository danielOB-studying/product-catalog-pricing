using Microsoft.EntityFrameworkCore;

namespace PricingApi.Repositories
{
    /// <summary>Implementación genérica de IRepository sobre Entity Framework Core.</summary>
    public class EFRepository<T> : IRepository<T>
        where T : class
    {
        private readonly DbSet<T> _set;

        public EFRepository(DbSet<T> set)
        {
            _set = set;
        }

        public IQueryable<T> Query()
        {
            return _set.AsQueryable();
        }

        public T? Find(int id)
        {
            return _set.Find(id);
        }

        public void Add(T entity)
        {
            _set.Add(entity);
        }

        public void Remove(T entity)
        {
            _set.Remove(entity);
        }
    }
}