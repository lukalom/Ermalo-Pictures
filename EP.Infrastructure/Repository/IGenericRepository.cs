using System.Linq.Expressions;
using EP.Infrastructure.IConfiguration;

namespace EP.Infrastructure.Repository
{
    public interface IGenericRepository<T, in TKey>
        where T : class, IEntity, IPrimaryKey<TKey> where TKey : notnull
    {
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);

        Task<IEnumerable<T>> GetAllFilterAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        Task<T?> GetSingleOrDefaultAsync(TKey id);

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

        IQueryable<T> Query();

        Task<bool> AddRangeAsync(IEnumerable<T> entity);

        Task<bool> AddAsync(T entity);

        bool Remove(T entity);

        bool RemoveRange(IEnumerable<T> entity);
    }
}
