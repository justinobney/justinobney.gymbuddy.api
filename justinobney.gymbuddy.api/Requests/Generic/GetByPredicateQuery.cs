using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Generic
{
    public class GetAllByPredicateQuery<TEntity> : IRequest<IQueryable<TEntity>>
    {
        public GetAllByPredicateQuery()
        {
        }

        public GetAllByPredicateQuery(Expression<Func<TEntity, bool>> predicate)
        {
            Predicate = predicate;
        }
        
        public Expression<Func<TEntity, bool>> Predicate { get; set; } = (entity => true);
    }
    
    [DoNotValidate]
    public class GetAllByPredicateQueryHandler<TEntity> : IRequestHandler<GetAllByPredicateQuery<TEntity>, IQueryable<TEntity>> where TEntity : class
    {
        private readonly IDbSet<TEntity> _dbSet;


        public GetAllByPredicateQueryHandler(IDbSet<TEntity> dbSet)
        {
            _dbSet = dbSet;
        }

        public IQueryable<TEntity> Handle(GetAllByPredicateQuery<TEntity> message)
        {
            return _dbSet.Where(message.Predicate);
        }
    }
}