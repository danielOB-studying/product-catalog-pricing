namespace PricingApi.Repositories
{
    /// <summary>Encapsula una transacción de base de datos para confirmar los cambios en bloque.</summary>
    public interface IUnitOfWork
    {
        IRepository<Models.PricingRule> PricingRules();
        void Save();
    }
}