using System.Collections.Generic;
using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Data.Posts;
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
        private readonly PostContentStategyFactory _postContentStategyFactory;

        public CreatePostCommandHandler(PostContentStategyFactory postContentStategyFactory)
        {
            _postContentStategyFactory = postContentStategyFactory;
        }

        public AsyncJob Handle(CreatePostCommand message)
        {
            return _postContentStategyFactory
                .GetByType(message.Content)
                .Handle(message);
        }
    }
}