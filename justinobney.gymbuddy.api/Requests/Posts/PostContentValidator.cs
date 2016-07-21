using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Posts;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class PostContentValidator : AbstractValidator<PostContent>
    {
        public PostContentValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty();
            
            Custom(content => content.Type > 0
                ? null
                : new ValidationFailure("Type", "'Type' should be one of 'TEXT' | 'IMAGE'."));
        }
    }
}