namespace CatalogApi.Repositories
{
    /// <summary>Contrato genérico de acceso a datos para desacoplar la lógica de negocio del ORM.</summary>
    public interface IRepository<T>
        where T : class
    {
        /// <summary>Expone la consulta subyacente para permitir Includes/Where sin romper el patrón.</summary>
        System.Linq.IQueryable<T> Query();

        /// <summary>Recupera una entidad por su clave primaria.</summary>
        T? Find(object id);

        void Add(T entity);
        void Remove(T entity);
    }
}