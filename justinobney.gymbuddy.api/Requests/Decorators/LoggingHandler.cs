using System;
using justinobney.gymbuddy.api.Helpers;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace justinobney.gymbuddy.api.Requests.Decorators
{
    public class DoNotLog : Attribute
    {
    }

    public class LoggingHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger _log;
        private readonly IRequestHandler<TRequest, TResponse> _inner;

        public LoggingHandler(ILogger log, IRequestHandler<TRequest, TResponse> inner)
        {
            _log = log;
            _inner = inner;
        }

        public TResponse Handle(TRequest message)
        {
            var type = message.GetType();
            _log.Information($"TRequest: {type.GetPrettyName()}");
            if (type.GenericTypeArguments.Length == 0)
            {
                _log.Information($"JSON {type.GetPrettyName()}: {JsonConvert.SerializeObject(message)}");
            }

            var response = _inner.Handle(message);

            _log.Information($"TResponse: {response.GetType().GetPrettyName()}");

            return response;
        }
    }
}