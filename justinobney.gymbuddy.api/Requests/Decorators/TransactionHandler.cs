using System;
using justinobney.gymbuddy.api.Data;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Decorators
{
    public class DoNotCommit : Attribute
    {
    }

    public class TransactionHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly AppContext _context;

        public TransactionHandler(
            IRequestHandler<TRequest, TResponse> inner,
            AppContext context
            )
        {
            _inner = inner;
            _context = context;
        }

        public TResponse Handle(TRequest message)
        {
            var result = _inner.Handle(message);
            _context.SaveChanges();
            return result;
        }
    }
}