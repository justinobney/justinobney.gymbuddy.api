using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbSet<TEntity> DbSet;

        protected readonly AppContext _dbContext;

        public BaseRepository(AppContext dbContext)
        {
            _dbContext = dbContext;
            DbSet = _dbContext.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public virtual async Task<TEntity> GetByIdAsync(long id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _dbContext.SetModified(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            DbSet.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            DbSet.Attach(entity);
            DbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}