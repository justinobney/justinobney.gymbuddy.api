using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class GetPostMetadataByIdsQuery : IRequest<IEnumerable<PostMetadata>>
    {
        public ICollection<long> PostIds { get; set; }
    }

    [DoNotValidate]
    [DoNotCommit]
    public class GetPostMetadataByIdsQueryHandler : IRequestHandler<GetPostMetadataByIdsQuery, IEnumerable<PostMetadata>>
    {
        private readonly IDbSet<Post> _posts;

        public GetPostMetadataByIdsQueryHandler(IDbSet<Post> posts)
        {
            _posts = posts;
        }

        public IEnumerable<PostMetadata> Handle(GetPostMetadataByIdsQuery message)
        {
            var posts = _posts
                .Include(x=>x.Comments)
                .Include(x=>x.Kudos)
                .Where(x => message.PostIds.Contains(x.Id));

            return posts.ToList().Select(x =>
            {
                var lastComment = "";

                if (x.Comments.Any())
                {
                    lastComment = x.Comments.OrderByDescending(y => y.Timestamp).First().Value;
                }

                return new PostMetadata
                {
                    LastComment = lastComment,
                    CommentCount = x.Comments.Count,
                    KudosCount = x.Kudos.Count
                };
            });
        }
    }

    public class PostMetadata
    {
        public string LastComment { get; set; }
        public int CommentCount { get; set; }
        public int  KudosCount { get; set; }
    }
}