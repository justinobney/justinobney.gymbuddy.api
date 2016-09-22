using System;
using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Requests.Posts;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Posts
{
    [TestFixture]
    public class GetPostMetaDataQueryTests:BaseTest
    {
        [Test]
        public void GetsCorrectPostMetaData()
        {
            var post1 = GetPost(_idCount++);
            var post2 = GetPost(_idCount++);

            Context.GetSet<Post>().Attach(post1);
            Context.GetSet<Post>().Attach(post2);

            var result = Mediator.Send(new GetPostMetadataByIdsQuery
            {
                PostIds = new List<long> {post1.Id, post2.Id}
            });

            result.Count().ShouldBe(2);

            var first = result.First();
            first.KudosCount.ShouldBe(1);
            first.CommentCount.ShouldBe(1);
            first.LastComment.ShouldBe($"Hi {post1.Id}");
        }

        private static int _idCount = 1;
        
        private static Post GetPost(int id)
        {
            return new Post
            {
                Id = id,
                Kudos = new List<PostKudos>
                {
                    GetPostKudos(id,123)
                },
                Comments = new List<PostComment>
                {
                    GetPostComment(id)
                }
            };
        }

        private static PostComment GetPostComment(int id)
        {
            return new PostComment
            {
                Id = _idCount++,
                PostId = id,
                UserId = 123,
                Timestamp = DateTime.UtcNow,
                Value = $"Hi {id}"
            };
        }

        private static PostKudos GetPostKudos(int id, int userId)
        {
            return new PostKudos
            {
                Id = _idCount++,
                UserId = userId,
                PostId = id
            };
        }
    }
}