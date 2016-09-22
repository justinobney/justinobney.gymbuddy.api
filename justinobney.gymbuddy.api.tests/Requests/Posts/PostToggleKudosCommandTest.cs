using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Requests.Posts;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Posts
{
    [TestFixture]
    public class PostToggleKudosCommandTest : BaseTest
    {
        public Post ThePost { get; set; }

        [SetUp]
        public void Setup()
        {
            ThePost = new Post
            {
                Id = 123,
                Kudos = new List<PostKudos>(),
                UserId = 123
            };
            Context.GetSet<Post>().Attach(ThePost);
        }

        [Test]
        public void ToggleKudosAddsKudos_DoesNotAllowMultipleKudosPerUser()
        {
            var _kudos = Context.GetSet<PostKudos>();
            Mediator.Send(new PostToggleKudosCommand
            {
                UserId = 123,
                PostId = ThePost.Id
            });

            _kudos.Count(x=>x.PostId == ThePost.Id).ShouldBe(1);
            AssertSaveChanges(1);

            Mediator.Send(new PostToggleKudosCommand
            {
                UserId = 123,
                PostId = ThePost.Id
            });

            _kudos.Count(x => x.PostId == ThePost.Id).ShouldBe(0);
            AssertSaveChanges(2);
        }

    }
}