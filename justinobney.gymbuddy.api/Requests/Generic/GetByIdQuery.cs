using System;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Generic
{
    public class GetByIdQuery<TEntity> : IRequest<TEntity> where TEntity : IEntity
    {
        public long Id { get; set; }
    }

    [DoNotValidate]
    public class GetByIdQueryHandler<TEntity> : IRequestHandler<GetByIdQuery<TEntity>, TEntity> where TEntity : class, IEntity
    {
        private readonly IDbSet<TEntity> _dbSet;


        public GetByIdQueryHandler(IDbSet<TEntity> dbSet)
        {
            _dbSet = dbSet;
        }

        TEntity IRequestHandler<GetByIdQuery<TEntity>, TEntity>.Handle(GetByIdQuery<TEntity> message)
        {
            var user = _dbSet.FirstOrDefault(x => x.Id == message.Id);
            if(user == null)
                throw new NullReferenceException("Entity does not exist");

            return user;
        }
    }
}