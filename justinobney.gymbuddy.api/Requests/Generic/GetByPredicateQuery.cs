using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Generic
{
    public class GetByPredicateQuery<TEntity> : IRequest<IQueryable<TEntity>>
    {
        public Expression<Func<TEntity, bool>> Predicate { get; set; } = (entity => true);
        public IEnumerable<Expression<Func<TEntity, object>>> Includes { get; set; } = new List<Expression<Func<TEntity, object>>>();
    }

    [DoNotValidate]
    public class GetByPredicateQueryHandler<TEntity> : IRequestHandler<GetByPredicateQuery<TEntity>, IQueryable<TEntity>> where TEntity : class
    {
        private readonly BaseRepository<TEntity> _repo;

        public GetByPredicateQueryHandler(BaseRepository<TEntity> repo)
        {
            _repo = repo;
        }

        public IQueryable<TEntity> Handle(GetByPredicateQuery<TEntity> message)
        {
            return message.Includes
                .Aggregate(
                    _repo.GetAll(),
                    (entities, expression) => entities.Include(expression)
                )
                .Where(message.Predicate);
        }
    }
}