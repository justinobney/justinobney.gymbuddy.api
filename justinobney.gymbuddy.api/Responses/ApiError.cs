using FluentValidation.Results;

namespace justinobney.gymbuddy.api.Responses
{
    public class ApiError
    {
        public ApiError(ValidationFailure validationFailure)
        {
            Property = validationFailure.PropertyName;
            Message = validationFailure.ErrorMessage;
        }

        public string Property { get; set; }
        public string Message { get; set; }
    }
}