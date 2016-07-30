using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Users;
using NSubstitute;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Users
{
    [TestFixture]
    public class CreateUserStreamSetupTests : BaseTest
    {
        public User CurrentUser { get; set; }
        public Gym DefaultGym { get; set; }

        [SetUp]
        public void Setup()
        {
            DefaultGym = new Gym {Id = 1, Name = "GloboGym"};
            CurrentUser = new User
            {
                Id = 1,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Gyms = new List<Gym> {DefaultGym},
                Name = "User",
                FacebookUserId = "1122334455"
            };
            Context.GetSet<User>().Attach(CurrentUser);
            Context.GetSet<Gym>().Attach(DefaultGym);
        }

        [Test]
        public void CreateUserStreamSetup_SchedulesStreamFollowJob()
        {
            var users = Context.GetSet<User>();
            var backgroundClient = Context.GetInstance<IBackgroundJobClient>();

            Context.Register<IPostRequestHandler<CreateUserCommand, User>, CreateUserStreamSetup>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateUserCommand, User>>();

            var request = new CreateUserCommand
            {
                Id = 1,
                Gender = Gender.Male,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Name = "User",
                FacebookUserId = "1122334455"
            };
            
            var user = users.First();

            handler.Notify(request, user);

            backgroundClient.Received(1).Create(
                Arg.Is<Job>(
                    x =>
                        x.Method.Name == "_FollowFeed"
                        && (string) x.Args[0] == "timeline"
                        && (string)x.Args[1] == "1"
                        && (string)x.Args[2] == "user_posts"
                        && (string)x.Args[3] == "1"
                    ),
                Arg.Any<IState>()
                );
        }
    }
}