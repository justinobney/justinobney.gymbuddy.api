using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Posts;
using justinobney.gymbuddy.api.tests.Helpers;
using NSubstitute;
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
        public void CreatePostCommand_WhenGivenPostContent_TypeText_CreatesPostAndReturnsMockJob()
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

        [Test]
        public void CreatePostCommand_WhenGivenPostContent_TypeImage_CreatesAsyncJob_AndQueuesBackgroundJob()
        {
            var posts = Context.GetSet<Post>();
            var asyncJobs = Context.GetSet<AsyncJob>();
            var context = Context.GetInstance<AppContext>();
            var backgroundClient = Context.GetInstance<IBackgroundJobClient>();

            var command = new CreatePostCommand
            {
                UserId = 1,
                Content = new List<PostContent>
                {
                    new PostContent
                    {
                        Type = PostType.Image,
                        Value = "1234456"
                    }
                }
            };

            var theJob = Mediator.Send(command);

            posts.Count().ShouldBe(0);
            asyncJobs.Count().ShouldBe(1);
            
            theJob.Id.ShouldBe(0); // 0 id is limitation of FakeDbSet `Add` from nuget
            theJob.ContentUrl.ShouldBe(null);
            theJob.StatusUrl.ShouldBe($"/api/jobs/{theJob.Id}");
            theJob.Status.ShouldBe(JobStatus.Pending);
            
            backgroundClient.Received().Create(
                Arg.Is<Job>(x => x.Method.Name == "ProcessImageContent"),
                Arg.Any<IState>()
            );

            context.Received(2).SaveChanges();
        }

        [Test]
        public void ImageContentStrategy_ProcessImageContent_DoesItsThing()
        {
            var asyncJobs = Context.GetSet<AsyncJob>();
            var posts = Context.GetSet<Post>();
            var context = Context.GetInstance<AppContext>();
            var strategy = Context.GetInstance<ImageContentStrategy>();
            var existingJob = new AsyncJob
            {
                Id = 1,
                Status = JobStatus.Pending,
                StatusUrl = "/api/jobs/1"
            };

            asyncJobs.Add(existingJob);

            var imageUri = "1234";
            var command = new CreatePostCommand
            {
                UserId = 1,
                Content = new List<PostContent>
                {
                    new PostContent
                    {
                        Type = PostType.Image,
                        Value = imageUri
                    }
                }
            };

            strategy.ProcessImageContent(command, existingJob.Id);

            var theJob = asyncJobs.First(x => x.Id == existingJob.Id);
            var thePost = posts.First();

            thePost.Contents.First().Value.ShouldBe($"url://{imageUri}");
            theJob.Status.ShouldBe(JobStatus.Complete);
            theJob.ContentUrl.ShouldBe($"/api/posts/{thePost.Id}");
            context.Received(2).SaveChanges();
        }
    }
}