using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class PostToggleKudosCommand : IRequest<Post>
    {
        public long PostId { get; set; }
        public long UserId { get; set; }
    }

    [DoNotValidate]
    public class PostToggleKudosCommandHandler : IRequestHandler<PostToggleKudosCommand, Post>
    {
        private readonly IDbSet<PostKudos> _kudos;
        private readonly IDbSet<Post> _posts;

        public PostToggleKudosCommandHandler(IDbSet<Post> posts, IDbSet<PostKudos> kudos)
        {
            _kudos = kudos;
            _posts = posts;
        }

        public Post Handle(PostToggleKudosCommand message)
        {
            var post = _posts
                .Include(x => x.Kudos)
                .First(x => x.Id == message.PostId);

            var userKudos = _kudos.FirstOrDefault(x =>
                x.UserId == message.UserId
                && x.PostId == message.PostId
            );

            if (userKudos == null)
            {
                _kudos.Add(new PostKudos
                {
                    UserId = message.UserId,
                    PostId = message.PostId
                });
            }
            else
            {
                _kudos.Remove(userKudos);
            }

            return post;
        }
    }
}