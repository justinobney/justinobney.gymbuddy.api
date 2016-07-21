using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Requests.Appointments.Create;
using NSubstitute;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Notifiers
{
    [TestFixture]
    public class CreateAppointmentNotifierTests : BaseTest
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
        public void CreateAppointmentNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();
            var notifier = Context.Container.GetInstance<IPushNotifier>();

            var gym = new Gym { Id = 1 };
            var user1 = new User
            {
                Id = 2,
                Gyms = new List<Gym> { gym },
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS"}
                }
            };

            var user2 = new User
            {
                Id = 3,
                Gyms = new List<Gym> { gym },
                Devices = new List<Device>
                {
                    new Device {PushToken = "654321", Platform = "Android"}
                }
            };

            users.Add(user1);
            users.Add(user2);

            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand { UserId = 1, GymId = 1 };
            var response = new Appointment { UserId = 1, User = new User { Name = "Justin" } };

            handler.Notify(request, response);
            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "New Appointment Available"),
                Arg.Any<IEnumerable<Device>>()
                );
        }

        [Test]
        public void CreateAppointmentNotifier_SendsGymUserNotification()
        {
            var users = Context.GetSet<User>();
            var notifier = Context.GetInstance<IPushNotifier>();

            var sameGymUser = new User
            {
                Id = 75,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Gyms = new List<Gym> { DefaultGym },
                Name = "SAME GYM USER",
                Devices = new List<Device>
                {
                    new Device { Platform = "iOS", PushToken = "98142"}
                },
                NewGymWorkoutNotifications = true
            };

            users.Add(sameGymUser);


            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand
            {
                UserId = CurrentUser.Id,
                GymId = DefaultGym.Id,
                Title = "Super WORKOUT",
                TimeSlots = new List<DateTime?> { DateTime.Now }
            };
            var response = new Appointment { UserId = 1, User = CurrentUser, GymId = DefaultGym.Id };

            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "New Appointment Available"),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "98142"))
                );
        }

        [Test]
        public void CreateAppointmentNotifier_DoesNotSendGymUserNotification()
        {
            var users = Context.GetSet<User>();
            var notifier = Context.GetInstance<IPushNotifier>();

            var sameGymUser = new User
            {
                Id = 99,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Gyms = new List<Gym> { DefaultGym },
                Name = "SAME GYM USER",
                Devices = new List<Device>
                {
                    new Device { Platform = "iOS", PushToken = "554467"}
                },
                NewGymWorkoutNotifications = false
            };

            users.Add(sameGymUser);


            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand
            {
                UserId = CurrentUser.Id,
                GymId = DefaultGym.Id,
                Title = "Super WORKOUT",
                TimeSlots = new List<DateTime?> { DateTime.Now }
            };
            var response = new Appointment { UserId = 1, User = CurrentUser, GymId = DefaultGym.Id };

            handler.Notify(request, response);

            notifier.DidNotReceive().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "New Appointment Available"),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "554467"))
                );
        }

        [Test]
        public void CreateAppointmentNotifier_SendsSquadUserNotification()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();
            var notifier = Context.GetInstance<IPushNotifier>();

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


            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand
            {
                UserId = CurrentUser.Id,
                GymId = DefaultGym.Id,
                Title = "Super WORKOUT",
                TimeSlots = new List<DateTime?> { DateTime.Now }
            };
            var response = new Appointment { UserId = 1, User = CurrentUser, GymId = DefaultGym.Id };

            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "New Appointment Available"),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "877777"))
                );

            notifier.DidNotReceive().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "New Appointment Available"),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "455655122"))
                );
        }

        [Test]
        public void CreateAppointmentNotifier_DoesNotSendSquadUserNotification()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();
            var notifier = Context.GetInstance<IPushNotifier>();

            var newNonGymUser = new User
            {
                Id = 65,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Name = "No GYm User",
                Devices = new List<Device>
                {
                    new Device { Platform = "iOS", PushToken = "65432111"}
                },
                NewSquadWorkoutNotifications = false
            };

            users.Add(newNonGymUser);
            friendships.Attach(new Friendship
            {
                UserId = CurrentUser.Id,
                FriendId = newNonGymUser.Id,
                Status = FriendshipStatus.Active,
                FriendshipKey = "1||65",
                Initiator = true
            });
            friendships.Attach(new Friendship
            {
                UserId = newNonGymUser.Id,
                FriendId = CurrentUser.Id,
                Status = FriendshipStatus.Active,
                FriendshipKey = "1||65"
            });


            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand
            {
                UserId = CurrentUser.Id,
                GymId = DefaultGym.Id,
                Title = "Super WORKOUT",
                TimeSlots = new List<DateTime?> { DateTime.Now }
            };
            var response = new Appointment { UserId = 1, User = CurrentUser, GymId = DefaultGym.Id };

            handler.Notify(request, response);

            notifier.DidNotReceive().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "New Appointment Available"),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "65432111"))
                );
        }

        [Test]
        public void CreateAppointmentNotifier_SilenceUserNotification()
        {
            var users = Context.GetSet<User>();
            var friendships = Context.GetSet<Friendship>();
            var notifier = Context.GetInstance<IPushNotifier>();

            var user = new User
            {
                Id = 10,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Gyms = new List<Gym> { DefaultGym },
                Name = "SAME GYM USER",
                Devices = new List<Device>
                {
                    new Device { Platform = "iOS", PushToken = "99999"}
                },
                NewGymWorkoutNotifications = true,
                NewSquadWorkoutNotifications = true,
                SilenceAllNotifications = true
            };

            users.Add(user);
            friendships.Attach(new Friendship
            {
                UserId = CurrentUser.Id,
                FriendId = user.Id,
                Status = FriendshipStatus.Active,
                FriendshipKey = "1||10",
                Initiator = true
            });
            friendships.Attach(new Friendship
            {
                UserId = user.Id,
                FriendId = CurrentUser.Id,
                Status = FriendshipStatus.Active,
                FriendshipKey = "1||10"
            });


            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand
            {
                UserId = CurrentUser.Id,
                GymId = DefaultGym.Id,
                Title = "Super WORKOUT",
                TimeSlots = new List<DateTime?> { DateTime.Now }
            };
            var response = new Appointment { UserId = 1, User = CurrentUser, GymId = DefaultGym.Id };

            handler.Notify(request, response);

            notifier.DidNotReceive().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "New Appointment Available"),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "99999"))
                );
        }

        [Test]
        public void AppointmentReminder_SchedulesTheReminder()
        {
            var users = Context.GetSet<User>();

            var sameGymUser = new User
            {
                Id = 75,
                Devices = new List<Device>
                {
                    new Device { Platform = "iOS", PushToken = "98142"}
                },
                NewGymWorkoutNotifications = true
            };

            users.Add(sameGymUser);

            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, AppointmentReminder>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();
            var backgroundClient = Context.GetInstance<IBackgroundJobClient>();

            var request = new CreateAppointmentCommand
            {
                UserId = CurrentUser.Id,
                GymId = DefaultGym.Id,
                Title = "Super WORKOUT",
                TimeSlots = new List<DateTime?> { DateTime.Now }
            };
            var response = new Appointment
            {
                Id = 2,
                UserId = 1,
                User = CurrentUser,
                GymId = DefaultGym.Id,
                TimeSlots = new List<AppointmentTimeSlot>
                {
                    new AppointmentTimeSlot
                    {
                        Time = DateTime.UtcNow.AddMinutes(121),
                        AppointmentId = 2
                    }
                }
            };

            handler.Notify(request, response);
            backgroundClient.Received().Create(
                Arg.Is<Job>(x => x.Method.Name == "SendNotification" && (long)x.Args[0] == 2),
                Arg.Any<IState>()
            );
        }

        [Test]
        public void AppointmentReminder_SkipsTheReminder_WhenAppointmentIsLessThanTwoHoursFromCreation()
        {
            var users = Context.GetSet<User>();

            var sameGymUser = new User
            {
                Id = 75,
                Devices = new List<Device>
                {
                    new Device { Platform = "iOS", PushToken = "98142"}
                },
                NewGymWorkoutNotifications = true
            };

            users.Add(sameGymUser);

            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, AppointmentReminder>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();
            var backgroundClient = Context.GetInstance<IBackgroundJobClient>();

            var request = new CreateAppointmentCommand
            {
                UserId = CurrentUser.Id,
                GymId = DefaultGym.Id,
                Title = "Super WORKOUT",
                TimeSlots = new List<DateTime?> { DateTime.Now }
            };
            var response = new Appointment
            {
                Id = 1,
                UserId = 1,
                User = CurrentUser,
                GymId = DefaultGym.Id,
                TimeSlots = new List<AppointmentTimeSlot>
                {
                    new AppointmentTimeSlot
                    {
                        Time = DateTime.UtcNow.AddMinutes(119),
                        AppointmentId = 1
                    }
                }
            };

            handler.Notify(request, response);
            backgroundClient.DidNotReceive().Create(
                Arg.Is<Job>(x => x.Method.Name == "SendNotification" && (long)x.Args[0] == 1),
                Arg.Any<IState>()
            );
        }

        [Test]
        public void AppointmentReminder_SendsTheReminderToConfirmedGuests()
        {
            var appts = Context.GetSet<Appointment>();
            var guests = Context.GetSet<AppointmentGuest>();
            var notifier = Context.GetInstance<IPushNotifier>();

            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, AppointmentReminder>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>() as AppointmentReminder;

            var appt = new Appointment
            {
                Id = 2,
                UserId = 1,
                User = CurrentUser,
                GymId = DefaultGym.Id,
                TimeSlots = new List<AppointmentTimeSlot>
                {
                    new AppointmentTimeSlot
                    {
                        Time = DateTime.UtcNow.AddMinutes(121),
                        AppointmentId = 2
                    }
                }
            };

            var guest1 = new AppointmentGuest
            {
                UserId = 2,
                AppointmentId = appt.Id,
                Status = AppointmentGuestStatus.Pending,
                User = new User
                {
                    Id = 2,
                    Devices = new List<Device> { new Device { PushToken = "123" } }
                }
            };

            var guest2 = new AppointmentGuest
            {
                UserId = 2,
                AppointmentId = appt.Id,
                Status = AppointmentGuestStatus.Confirmed,
                User = new User
                {
                    Id = 2,
                    Devices = new List<Device> { new Device { PushToken = "321" } }
                }
            };

            appts.Attach(appt);
            guests.Attach(guest1);
            guests.Attach(guest2);

            handler.SendNotification(appt.Id);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "Upcoming Workout"),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "321"))
                );

            notifier.DidNotReceive().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "Upcoming Workout"),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123"))
                );
        }
    }
}