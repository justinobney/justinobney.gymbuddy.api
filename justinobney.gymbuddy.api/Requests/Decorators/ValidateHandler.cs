using System;
using FluentValidation;
using MediatR;

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

        public ValidateHandler(
            IRequestHandler<TRequest, TResponse> inner,
            AbstractValidator<TRequest> validator
            )
        {
            _inner = inner;
            _validator = validator;
        }

        public TResponse Handle(TRequest message)
        {
            var result = _validator.Validate(message);
            if (result.IsValid)
            {
                return _inner.Handle(message);
            }

            throw new ValidationException(result.Errors);
        }
    }
}