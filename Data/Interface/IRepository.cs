using System.Linq.Expressions;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null);

        Task<T?> GetByIdAsync(
            int id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}