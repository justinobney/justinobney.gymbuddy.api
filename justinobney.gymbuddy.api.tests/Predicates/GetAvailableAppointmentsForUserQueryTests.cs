using System;
using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Appointments;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Predicates
{
    [TestFixture]
    public class GetAvailableAppointmentsForUserQueryTests : BaseTest
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
        public void GetAvailableAppointmentsForUserQuery_FindsNonGymSquadUsers()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();
            var appointments = Context.GetSet<Appointment>();

            var newNonGymUser = new User
            {
                Id = 57,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Name = "No GYm User",
                Devices = new List<Device>
                {
                    new Device { Platform = "iOS", PushToken = "877777"}
                },
                NewSquadWorkoutNotifications = true
            };

            var newNonSquadUser = new User
            {
                Id = 455,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Name = "MEOW",
                Devices = new List<Device>
                {
                    new Device { Platform = "iOS", PushToken = "455655122"}
                },
                NewSquadWorkoutNotifications = true
            };

            users.Add(newNonGymUser);
            users.Add(newNonSquadUser);

            friendships.Attach(new Friendship
            {
                UserId = CurrentUser.Id,
                FriendId = newNonGymUser.Id,
                Status = FriendshipStatus.Active,
                FriendshipKey = "1||57",
                Initiator = true
            });
            friendships.Attach(new Friendship
            {
                UserId = newNonGymUser.Id,
                FriendId = CurrentUser.Id,
                Status = FriendshipStatus.Active,
                FriendshipKey = "1||57"
            });

            friendships.Attach(new Friendship
            {
                UserId = CurrentUser.Id,
                FriendId = newNonSquadUser.Id,
                Status = FriendshipStatus.Pending,
                FriendshipKey = "1||455",
                Initiator = true
            });
            friendships.Attach(new Friendship
            {
                UserId = newNonSquadUser.Id,
                FriendId = CurrentUser.Id,
                Status = FriendshipStatus.Pending,
                FriendshipKey = "1||455"
            });

            appointments.Attach(new Appointment
            {
                TimeSlots = new List<AppointmentTimeSlot>
                {
                    new AppointmentTimeSlot
                    {
                        AppointmentId = 1,
                        Time = DateTime.UtcNow.AddMinutes(30)
                    }
                },
                Location = "Foo",
                UserId = CurrentUser.Id,
                User = CurrentUser,
                Status = AppointmentStatus.AwaitingGuests
            });

            Mediator.Send(new GetAvailableAppointmentsForUserQuery
            {
                UserId = newNonGymUser.Id,
                GymIds = new List<long>()
            }).Count().ShouldBe(1);

            Mediator.Send(new GetAvailableAppointmentsForUserQuery
            {
                UserId = newNonSquadUser.Id,
                GymIds = new List<long>()
            }).Count().ShouldBe(0);

        }
    }
}