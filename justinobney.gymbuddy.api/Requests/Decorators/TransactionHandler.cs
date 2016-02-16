using System;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Data;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Decorators
{
    public class Commit : Attribute
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

    public class TransactionHandlerAsync<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _inner;
        private readonly AppContext _context;

        public TransactionHandlerAsync(
            IAsyncRequestHandler<TRequest, TResponse> inner,
            AppContext context
            )
        {
            _inner = inner;
            _context = context;
        }


        async Task<TResponse> IAsyncRequestHandler<TRequest, TResponse>.Handle(TRequest message)
        {
            var result = await _inner.Handle(message);
            await _context.SaveChangesAsync();
            return result;
        }
    }
}