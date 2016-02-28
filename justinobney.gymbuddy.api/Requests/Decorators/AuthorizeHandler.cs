using System;
using justinobney.gymbuddy.api.Exceptions;
using justinobney.gymbuddy.api.Helpers;
using justinobney.gymbuddy.api.Interfaces;
using MediatR;
using Serilog;

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
        private readonly ILogger _log;

        public AuthorizeHandler(
            IRequestHandler<TRequest, TResponse> inner,
            IAuthorizer<TRequest> authorizer,
            ILogger log
            )
        {
            _inner = inner;
            _authorizer = authorizer;
            _log = log;
        }

        public TResponse Handle(TRequest message)
        {
            _log.Information($"Authorizing TRequest: {message.GetType().GetPrettyName()}");
            if (_authorizer.Authorize(message))
            {
                _log.Information($"Authorizing TRequest: {message.GetType().GetPrettyName()} PASSED");
                return _inner.Handle(message);
            }

            _log.Information($"Authorizing TRequest: {message.GetType().GetPrettyName()} FAILED");
            throw new AuthorizationException();
        }
    }
}