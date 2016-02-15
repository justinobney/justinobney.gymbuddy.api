using System.Data.Entity;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Generic
{
    public class GetByIdQuery<TEntity> : IAsyncRequest<TEntity> where TEntity : IEntity
    {
        public long Id { get; set; }
    }

    [DoNotValidate]
    public class GetByIdQueryHandler<TEntity> : IAsyncRequestHandler<GetByIdQuery<TEntity>, TEntity> where TEntity : class, IEntity
    {
        private readonly IDbSet<TEntity> _dbSet;


        public GetByIdQueryHandler(IDbSet<TEntity> dbSet)
        {
            _dbSet = dbSet;
        }

        Task<TEntity> IAsyncRequestHandler<GetByIdQuery<TEntity>, TEntity>.Handle(GetByIdQuery<TEntity> message)
        {
            return _dbSet.FirstAsync(x => x.Id == message.Id);
        }
    }
}