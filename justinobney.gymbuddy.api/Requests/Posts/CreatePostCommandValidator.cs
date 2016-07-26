using FluentValidation;
using justinobney.gymbuddy.api.Data.Posts;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidator(IValidator<PostContent> contentValidator)
        {
            RuleFor(x => x.Content.Count)
                .GreaterThan(0);

            RuleFor(x => x.UserId)
                .GreaterThan(0);

            RuleFor(x => x.Content).SetCollectionValidator(contentValidator);
        }
    }
}