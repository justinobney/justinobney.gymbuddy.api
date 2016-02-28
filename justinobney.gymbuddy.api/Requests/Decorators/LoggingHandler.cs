using System;
using System.Diagnostics;
using justinobney.gymbuddy.api.Helpers;
using MediatR;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

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
            _log.Information($"TRequest: {message.GetType().GetPrettyName()}");
            if (!message.GetType().ContainsGenericParameters)
            {
                _log.Information($"JSON {JsonConvert.SerializeObject(message)}");
            }
            var response = _inner.Handle(message);
            _log.Information($"TResponse: {response.GetType().GetPrettyName()}");

            return response;
        }
    }
}