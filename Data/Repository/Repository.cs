using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Repository;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null)
    {
        IQueryable<T> query = _dbSet;

        if (include != null)
        {
            query = include.Compile()(query);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id, Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null)
    {
        IQueryable<T> query = _dbSet;

        if (include != null)
        {
            query = include.Compile()(query);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}