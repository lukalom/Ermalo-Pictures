using System.Linq.Expressions;
using EP.Infrastructure.Data;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repository
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey>
        where T : class, IEntity, IPrimaryKey<TKey> where TKey : notnull
    {
        protected readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);

            if (includeProperties != null)
            {
                foreach (var includeProp in
                         includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllFilterAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.ToListAsync();
        }

        //Better practice Returning IQueryable
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return dbSet.Where(expression).AsQueryable();
            //.AsNoTracking();
        }

        public async Task<T?> GetSingleOrDefaultAsync(TKey id)
        {
            //if there is more than one item Throw Err
            return await dbSet.SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public IQueryable<T> Query()
        {
            return dbSet.AsQueryable();
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entity)
        {
            await dbSet.AddRangeAsync(entity);
            return true;
        }

        public virtual bool Remove(T entity)
        {
            dbSet.Remove(entity);
            return true;
        }

        public virtual bool RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
            return true;
        }
    }
}
