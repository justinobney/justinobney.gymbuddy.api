using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> GetByIdAsync(long id);

        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetAll();

        Task UpdateAsync(TEntity entity);

        Task InsertAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}