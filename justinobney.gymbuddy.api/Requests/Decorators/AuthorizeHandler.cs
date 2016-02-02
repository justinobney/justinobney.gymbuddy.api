using System;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Exceptions;
using justinobney.gymbuddy.api.Interfaces;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Decorators
{

    public class Authorize : Attribute
    {
    }

    public class AuthorizeHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IAuthorizer<TRequest> _authorizer;

        public AuthorizeHandler(
            IRequestHandler<TRequest, TResponse> inner,
            IAuthorizer<TRequest> authorizer
            )
        {
            _inner = inner;
            _authorizer = authorizer;
        }

        public TResponse Handle(TRequest message)
        {
            if (_authorizer.Authorize(message))
            {
                return _inner.Handle(message);
            }
            
            throw new AuthorizationException();
        }
    }

    public class AuthorizeHandlerAsync<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _inner;
        private readonly IAuthorizer<TRequest> _authorizer;

        public AuthorizeHandlerAsync(
            IAsyncRequestHandler<TRequest, TResponse> inner,
            IAuthorizer<TRequest> authorizer
            )
        {
            _inner = inner;
            _authorizer = authorizer;
        }

        public async Task<TResponse> Handle(TRequest message)
        {
            if (_authorizer.Authorize(message))
            {
                return await _inner.Handle(message);
            }

            throw new AuthorizationException();
        }
    }
}