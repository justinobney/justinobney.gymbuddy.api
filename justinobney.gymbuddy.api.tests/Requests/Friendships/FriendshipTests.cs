using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Frienships;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Friendships
{
    [TestFixture]
    public class FriendshipTests : BaseTest
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
        public void RequestFriendshipCreates2WayFriendship()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();

            var friend = new User
            {
                Id = 2,
                Name = "Bobcat"
            };

            users.Attach(friend);

            var command = new RequestFriendshipCommand
            {
                UserId = CurrentUser.Id,
                FriendId = friend.Id
            };

            Mediator.Send(command);

            friendships.Count(
                x =>
                    x.UserId == CurrentUser.Id
                    && x.Status == FriendshipStatus.Pending
                    && x.Initiator == true
                ).ShouldBe(1);

            friendships.Count(
                x =>
                    x.UserId == friend.Id
                    && x.Status == FriendshipStatus.Pending
                    && x.Initiator == false
                ).ShouldBe(1);
        }

        [Test]
        public void RequestFriendshipPreventsDuplicates()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();
            
            var friend = new User
            {
                Id = 2,
                Name = "Bobcat"
            };

            users.Attach(friend);

            friendships.Attach(new Friendship {FriendshipKey = "1||2"});
            friendships.Attach(new Friendship {FriendshipKey = "1||2"});

            var command = new RequestFriendshipCommand
            {
                UserId = CurrentUser.Id,
                FriendId = friend.Id
            };

            Action action = () => Mediator.Send(command);

            action.ShouldThrow<ValidationException>();
        }

        [Test]
        public void GetFriendshipReturnsCorrectFriendship()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();

            var friend = new User
            {
                Id = 2,
                Name = "Bobcat"
            };

            users.Attach(friend);

            friendships.Attach(new Friendship
            {
                UserId = CurrentUser.Id,
                FriendId = friend.Id,
                Status = FriendshipStatus.Pending,
                FriendshipKey = "1||2"
            });
            friendships.Attach(new Friendship
            {
                UserId = friend.Id,
                FriendId = CurrentUser.Id,
                Status = FriendshipStatus.Pending,
                FriendshipKey = "1||2"
            });

            var command = new GetFriendshipQuery
            {
                UserId = CurrentUser.Id,
                FriendId = friend.Id
            };

            var friendship = Mediator.Send(command);
            friendship.Status.ShouldBe(FriendshipStatus.Pending);
        }

        [Test]
        public void GetFriendshipReturnsNullIfNoHistoryFriendship()
        {
            var users = Context.GetSet<User>();

            var friend = new User
            {
                Id = 2,
                Name = "Bobcat"
            };

            users.Attach(friend);
            
            var command = new GetFriendshipQuery
            {
                UserId = CurrentUser.Id,
                FriendId = friend.Id
            };

            var friendship = Mediator.Send(command);
            friendship.Status.ShouldBe(FriendshipStatus.Unknown);
        }

        [Test]
        public void GetFriendshipReturnsRequestOnlyForNonInitiatorFriendship()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();

            var friend = new User
            {
                Id = 2,
                Name = "Bobcat"
            };

            users.Attach(friend);

            friendships.Attach(new Friendship
            {
                UserId = CurrentUser.Id,
                FriendId = friend.Id,
                Status = FriendshipStatus.Pending,
                FriendshipKey = "1||2",
                Initiator = true
            });
            friendships.Attach(new Friendship
            {
                UserId = friend.Id,
                FriendId = CurrentUser.Id,
                Status = FriendshipStatus.Pending,
                FriendshipKey = "1||2"
            });

            var userToFriendRequest = new GetAllFriendshipRequestsQuery
            {
                UserId = CurrentUser.Id
            };
            var userFriendshipRequests = Mediator.Send(userToFriendRequest);
            userFriendshipRequests.Count().ShouldBe(0);

            var friendToUserRequest = new GetAllFriendshipRequestsQuery
            {
                UserId = friend.Id
            };
            var friendFriendshipRequests = Mediator.Send(friendToUserRequest);
            friendFriendshipRequests.Count().ShouldBe(1);
        }

        [Test]
        public void ConfirmFriendshipSetsStatusForBothDirections()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();

            var friend = new User
            {
                Id = 2,
                Name = "Bobcat"
            };

            users.Attach(friend);

            friendships.Attach(new Friendship
            {
                UserId = CurrentUser.Id,
                FriendId = friend.Id,
                Status = FriendshipStatus.Pending,
                FriendshipKey = "1||2"
            });
            friendships.Attach(new Friendship
            {
                UserId = friend.Id,
                FriendId = CurrentUser.Id,
                Status = FriendshipStatus.Pending,
                FriendshipKey = "1||2"
            });

            var command = new ConfirmFriendshipCommand
            {
                UserId = friend.Id,
                FriendId = CurrentUser.Id
            };

            var friendship = Mediator.Send(command);
            friendship.Status.ShouldBe(FriendshipStatus.Active);

            friendships.Count(x => x.UserId == CurrentUser.Id && x.Status == FriendshipStatus.Active).ShouldBe(1);
            friendships.Count(x => x.UserId == friend.Id && x.Status == FriendshipStatus.Active).ShouldBe(1);
        }
    }
}