using System;
using System.Diagnostics;
using System.Threading.Tasks;
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

    public class LoggingHandlerAsync<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _inner;

        public LoggingHandlerAsync(IAsyncRequestHandler<TRequest, TResponse> inner)
        {
            _inner = inner;
        }

        public async Task<TResponse> Handle(TRequest message)
        {
            Debug.WriteLine("Request: {0}", message);
            var response = await _inner.Handle(message);
            Debug.WriteLine("Response: {0}", response);

            return response;
        }
    }
}