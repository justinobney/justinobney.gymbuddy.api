using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Friendships;
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
                Name = "User",
                FacebookUserId = "1122334455"
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

        [Test]
        public void ConfirmFriendshipFromInitiatorFails()
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

            var command = new ConfirmFriendshipCommand
            {
                UserId = CurrentUser.Id,
                FriendId = friend.Id
            };

            Action action = () => Mediator.Send(command);
            action.ShouldThrow<ValidationException>();
        }

        [Test]
        public void GetFriendshipsByFacebookIdsHandlesNullFacebookIdCollection()
        {
            var command = new GetFriendshipsByFacebookIdsCommand
            {
                UserId = CurrentUser.Id,
                FbIds = null
            };

            Action action = () => Mediator.Send(command);
            action.ShouldThrow<ValidationException>();
        }

        [Test]
        public void GetFriendshipsByFacebookIdsReturnsOnlyDistinctFacebookIdInformation()
        {
            var users = Context.GetSet<User>();

            var friend = new User
            {
                Id = 2,
                Name = "Bobcat",
                FacebookUserId = "1337"
            };

            users.Attach(friend);
            
            var command = new GetFriendshipsByFacebookIdsCommand
            {
                UserId = friend.Id,
                FbIds = new List<string> { "1337", "1337", "1447" }
            };

            var facebookFriendshipListings = Mediator.Send(command);

            facebookFriendshipListings.Count(x => x.FacebookUserId == "1337").ShouldBe(1);
            facebookFriendshipListings.Count.ShouldBe(2);
        }

        [Test]
        public void GetFriendshipsByFacebookIdsStatusUnknownFacebookUser()
        {
            var users = Context.GetSet<User>();
            var user = new User
            {
                Id = 2,
                Name = "Bobcat",
                FacebookUserId = "1337"
            };
            users.Attach(user);

            var command = new GetFriendshipsByFacebookIdsCommand
            {
                UserId = CurrentUser.Id,
                FbIds = new List<string> { "1337", "1446", "s3dkl" }
            };

            var facebookFriendshipListings = Mediator.Send(command);
            facebookFriendshipListings.Count(x => x.Status == FacebookFriendshipStatus.UnknownFacebookUser).ShouldBe(2);
            facebookFriendshipListings.Count(x => x.UserId == 0).ShouldBe(2);
        }

        [Test]
        public void GetFriendshipsByFacebookIdsStatusKnownUserNotInSquad()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();

            var friend = new User
            {
                Id = 2,
                Name = "Bobcat",
                FacebookUserId = "1337"
            };
            var nonFriend = new User
            {
                Id = 3,
                Name = "MONSTAR",
                FacebookUserId = "0"
            };

            users.Attach(friend);
            users.Attach(nonFriend);

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
                FriendshipKey = "1||2",
                Initiator = false
            });

            var command = new GetFriendshipsByFacebookIdsCommand
            {
                UserId = CurrentUser.Id,
                FbIds = new List<string> { "1337", "0", "1555" }
            };

            var facebookFriendshipListings = Mediator.Send(command);
            facebookFriendshipListings.Count(x => x.Status == FacebookFriendshipStatus.KnownUserNonSquadMember).ShouldBe(2);
        }

        [Test]
        public void GetFriendshipsByFacebookIdsStatusInSquadUser()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();

            var friend = new User
            {
                Id = 2,
                Name = "Bobcat",
                FacebookUserId = "1337"
            };

            users.Attach(friend);

            friendships.Attach(new Friendship
            {
                UserId = CurrentUser.Id,
                FriendId = friend.Id,
                Status = FriendshipStatus.Active,
                FriendshipKey = "1||2",
                Initiator = true
            });
            friendships.Attach(new Friendship
            {
                UserId = friend.Id,
                FriendId = CurrentUser.Id,
                Status = FriendshipStatus.Active,
                FriendshipKey = "1||2"
            });


            var command = new GetFriendshipsByFacebookIdsCommand
            {
                UserId = CurrentUser.Id,
                FbIds = new List<string> { "1337", "1446" }
            };

            var facebookFriendshipListings = Mediator.Send(command);
            facebookFriendshipListings.Count(x => x.Status == FacebookFriendshipStatus.SquadMember).ShouldBe(1);
        }

        [Test]
        public void GetFriendshipsByFacebookIdsStatusSelf()
        {
            var command = new GetFriendshipsByFacebookIdsCommand
            {
                UserId = CurrentUser.Id,
                FbIds = new List<string> { "1122334455", "1337" }
            };

            var facebookFriendshipListings = Mediator.Send(command);
            facebookFriendshipListings.Count(x => x.Status == FacebookFriendshipStatus.Self).ShouldBe(1);
        }
    }
}