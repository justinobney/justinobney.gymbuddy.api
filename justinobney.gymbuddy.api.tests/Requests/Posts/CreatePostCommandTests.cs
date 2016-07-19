using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Posts;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Posts
{
    [TestFixture]
    public class CreatePostCommandTests : BaseTest
    {
        public User CurrentUser { get; set; }
        public Gym DefaultGym { get; set; }

        [SetUp]
        public void Setup()
        {
            DefaultGym = new Gym { Id = 1, Name = "GloboGym" };
            CurrentUser = new User
            {
                Id = 1,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Gyms = new List<Gym> { DefaultGym },
                Name = "User"
            };
            Context.GetSet<User>().Attach(CurrentUser);
            Context.GetSet<Gym>().Attach(DefaultGym);
        }

        [Test]
        public void CreatePostCommand_ThrowsValidationException_WhenNoContent()
        {
            Action foo = () => Mediator.Send(new CreatePostCommand());
            foo.ShouldThrow<ValidationException>();
        }

        [Test]
        public void CreatePostCommand_ThrowsValidationException_WhenContentInvalid()
        {
            Action foo = () => Mediator.Send(new CreatePostCommand
            {
                Content = new List<PostContent>
                {
                    new PostContent()
                }
            });
            foo.ShouldThrow<ValidationException>(@"Validation failed: 
 -- 'User Id' must be greater than '0'.
 -- 'Value' should not be empty.
 -- 'Type' should be one of 'TEXT' | 'IMAGE'.");
        }

        [Test]
        public void CreatePostCommand_WhenGivenPostContentTypeText_CreatesPostAndReturnsMockJob()
        {
            var posts = Context.GetSet<Post>();
            var asyncJobs = Context.GetSet<AsyncJob>();

            var command = new CreatePostCommand
            {
                UserId = 1,
                Content = new List<PostContent>
                {
                    new PostContent
                    {
                        Type = PostType.Text,
                        Value = "I made a post"
                    }
                }
            };

            var theJob = Mediator.Send(command);

            posts.Count().ShouldBe(1);
            asyncJobs.Count().ShouldBe(0);

            var thePost = posts.First();
            thePost.Contents.Count.ShouldBe(1);

            theJob.Id.ShouldBe(0);
            theJob.ContentUrl.ShouldBe($"/api/posts/{thePost.Id}");
            theJob.StatusUrl.ShouldBe(null);
            theJob.Status.ShouldBe(JobStatus.Complete);
        }
    }
}