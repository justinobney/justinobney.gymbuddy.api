using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
using WebGrease.Css.Extensions;

namespace justinobney.gymbuddy.api.Requests.Generic
{
    public class GetByPredicateQuery<TEntity> : IRequest<IQueryable<TEntity>>
    {
        public Expression<Func<TEntity, bool>> Predicate { get; set; }
        public IEnumerable<Expression<Func<TEntity, object>>> Includes  { get; set; }
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
            var query = _repo.GetAll();
            message.Includes.ForEach(include => query = query.Include(include));
            return query.Where(message.Predicate);
        }
    }
}