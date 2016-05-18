using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Helpers
{
    public class EntityCopier<T> where T : class, IEntity
    {
        private readonly DbContext _context;
        private readonly IDbSet<T> _set;

        public EntityCopier(DbContext context, IDbSet<T> set)
        {
            _context = context;
            _set = set;
        }

        public void Copy(int id, params Expression<Func<T, object>>[] includes)
        {
            var clone = includes
                .Aggregate(
                    _set.AsQueryable(),
                    (entities, expression) => entities.Include(expression)
                )
                .AsNoTracking()
                .Single(x => x.Id == id);
            
            _set.Add(clone);
            _context.SaveChanges();
        }
    }
}