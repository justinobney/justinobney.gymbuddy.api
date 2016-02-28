using System;
using System.Collections.Generic;
using System.Diagnostics;
using justinobney.gymbuddy.api.Helpers;
using justinobney.gymbuddy.api.Interfaces;
using MediatR;
using Serilog;

namespace justinobney.gymbuddy.api.Requests.Decorators
{
    public class PostRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IEnumerable<IPostRequestHandler<TRequest, TResponse>> _notifiers;
        private readonly ILogger _log;

        public PostRequestHandler(
            IRequestHandler<TRequest, TResponse> inner,
            IEnumerable<IPostRequestHandler<TRequest, TResponse>> notifiers,
            ILogger log
        )
        {
            _inner = inner;
            _notifiers = notifiers;
            _log = log;
        }

        public TResponse Handle(TRequest message)
        {
            var response = _inner.Handle(message);

            foreach (var notifier in _notifiers)
            {
                try
                {
                    _log.Information($"Notifying TRequest: {message.GetType().GetPrettyName()} ::: {notifier.GetType().GetPrettyName()}");
                    notifier.Notify(message, response);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }
            }
            
            return response;
        }
    }
}