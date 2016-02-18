using System;
using System.Diagnostics;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Decorators
{
    public class DoNotLog : Attribute
    {
    }

    public class LoggingHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;

        public LoggingHandler(IRequestHandler<TRequest, TResponse> inner)
        {
            _inner = inner;
        }

        public TResponse Handle(TRequest message)
        {
            Debug.WriteLine("Request: {0}", message);
            var response = _inner.Handle(message);
            Debug.WriteLine("Response: {0}", response);

            return response;
        }
    }
}