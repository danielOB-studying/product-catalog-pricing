namespace CatalogApi.Repositories
{
    /// <summary>Encapsula una transacción de base de datos para confirmar los cambios en bloque.</summary>
    public interface IUnitOfWork
    {
        IRepository<Models.Category> Categories();
        IRepository<Models.Product> Products();
        IRepository<Models.ApplicationUser> ApplicationUsers();
        void Save();
    }
}