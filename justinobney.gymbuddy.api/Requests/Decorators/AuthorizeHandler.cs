using System;
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
}