using System;
using System.Linq;
using System.Linq.Expressions;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Generic
{
    public class GetAllByPredicateQuery<TEntity> : IRequest<IQueryable<TEntity>>
    {
        public Expression<Func<TEntity, bool>> Predicate { get; set; } = (entity => true);
    }

    [DoNotValidate]
    public class GetAllByPredicateQueryHandler<TEntity> : IRequestHandler<GetAllByPredicateQuery<TEntity>, IQueryable<TEntity>> where TEntity : class
    {
        private readonly BaseRepository<TEntity> _repo;

        public GetAllByPredicateQueryHandler(BaseRepository<TEntity> repo)
        {
            _repo = repo;
        }

        public IQueryable<TEntity> Handle(GetAllByPredicateQuery<TEntity> message)
        {
            return _repo.GetAll().Where(message.Predicate);
        }
    }
}