using System;
using FluentValidation;
using justinobney.gymbuddy.api.Helpers;
using MediatR;
using Serilog;

namespace justinobney.gymbuddy.api.Requests.Decorators
{

    public class DoNotValidate : Attribute
    {
    }


    public class ValidateHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly AbstractValidator<TRequest> _validator;
        private readonly ILogger _log;

        public ValidateHandler(
            IRequestHandler<TRequest, TResponse> inner,
            AbstractValidator<TRequest> validator,
            ILogger log
            )
        {
            _inner = inner;
            _validator = validator;
            _log = log;
        }

        public TResponse Handle(TRequest message)
        {
            _log.Information($"Validating TRequest: {message.GetType().GetPrettyName()}");
            var result = _validator.Validate(message);
            if (result.IsValid)
            {
                _log.Information($"Validating TRequest: {message.GetType().GetPrettyName()} PASSED");
                return _inner.Handle(message);
            }

            _log.Information($"Validating TRequest: {message.GetType().GetPrettyName()} FAILED");
            throw new ValidationException(result.Errors);
        }
    }
}