using System.Collections.Generic;
using System.Data.Entity;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Enums;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class CreatePostCommand : IRequest<AsyncJob>
    {
        public ICollection<PostContent> Content { get; set; } = new List<PostContent>();
        public long UserId { get; set; }
    }

    
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, AsyncJob>
    {
        private readonly IDbSet<Post> _posts;
        private readonly AppContext _context;

        public CreatePostCommandHandler(
            IDbSet<Post> posts,
            AppContext context
        )
        {
            _posts = posts;
            _context = context;
        }

        public AsyncJob Handle(CreatePostCommand message)
        {
            var post = new Post
            {
                Contents = message.Content,
                UserId = message.UserId
            };

            _posts.Add(post);
            _context.SaveChanges();

            var job = new AsyncJob
            {
                Id = 0,
                Status = JobStatus.Complete,
                StatusUrl = null,
                ContentUrl = $"/api/posts/{post.Id}", //TODO: make URL factory
            };

            return job;
        }
    }

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