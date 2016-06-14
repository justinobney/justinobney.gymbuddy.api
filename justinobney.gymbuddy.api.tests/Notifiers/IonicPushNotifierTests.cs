using System;
using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Requests.Appointments.AddAppointmentGuest;
using justinobney.gymbuddy.api.Requests.Appointments.Comments;
using justinobney.gymbuddy.api.Requests.Appointments.Confirm;
using justinobney.gymbuddy.api.Requests.Appointments.Delete;
using justinobney.gymbuddy.api.Requests.Appointments.Edit;
using justinobney.gymbuddy.api.Requests.Appointments.RemoveAppointmentGuest;
using justinobney.gymbuddy.api.Requests.Friendships;
using justinobney.gymbuddy.api.tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Notifiers
{
    [TestFixture]
    public class IonicPushNotifierTests : BaseTest
    {
        [Test]
        public void NotifierShouldSerializeCorrectly()
        {
            var payload = new FooPayload {Type = "Foo", Foo = "Bar"};
            var notification = new NotificationPayload(payload)
            {
                Alert = "Alert",
                Title = "Title"
            };

            var notifier = new IonicPushNotification(notification)
            {
                Tokens = new List<string> { "123" }
            };

            var serializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var expected = "{\"tokens\":[\"123\"],\"production\":false,\"notification\":{\"alert\":\"Alert\",\"title\":\"Title\",\"android\":{\"sound\":\"default\",\"payload\":{\"foo\":\"Bar\",\"type\":\"Foo\",\"appointmentId\":0}},\"ios\":{\"sound\":\"default\",\"payload\":{\"foo\":\"Bar\",\"type\":\"Foo\",\"appointmentId\":0},\"badge\":null}}}";
            JsonConvert.SerializeObject(notifier, serializationSettings).ShouldBe(expected);

            var expected2 = "{\"tokens\":null,\"production\":false,\"notification\":{\"alert\":null,\"title\":null,\"android\":{\"sound\":\"default\",\"payload\":null},\"ios\":{\"sound\":\"default\",\"payload\":null,\"badge\":null}}}";
            JsonConvert.SerializeObject(new IonicPushNotification(new NotificationPayload(null)), serializationSettings).ShouldBe(expected2);
        }
        
        [Test]
        public void AddAppointmentGuestPushNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();
            
            var owner = new User
            {
                Id = 1,
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS"}
                }
            };
            users.Add(owner);

            users.Add(new User
            {
                Id = 2
            });

            appts.Add(new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id
            });

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<AddAppointmentGuestCommand, AppointmentGuest>, AddAppointmentGuestPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AddAppointmentGuestCommand, AppointmentGuest>>();

            var request = new AddAppointmentGuestCommand { AppointmentId = 1, UserId = 2 };
            var response = new AppointmentGuest { UserId = 1, User = new User { Name = "Justin" } };
            
            handler.Notify(request, response);
            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "Appointment Guest Request" && x.Ios.Payload.Type == NofiticationTypes.AddAppointmentGuest),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );

            ConfigIoC();
        }

        [Test]
        public void RemoveAppointmentGuestPushNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();

            var owner = new User
            {
                Id = 1,
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS"}
                }
            };
            users.Add(owner);

            users.Add(new User
            {
                Id = 2
            });

            appts.Add(new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id
            });

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<RemoveAppointmentGuestCommand, Appointment>, RemoveAppointmentGuestPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<RemoveAppointmentGuestCommand, Appointment>>();

            var request = new RemoveAppointmentGuestCommand { AppointmentId = 1, UserId = 2 };
            var response = new Appointment { UserId = 1, User = new User { Name = "Justin" } };

            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "Appointment Guest Left :(" && x.Ios.Payload.Type == NofiticationTypes.RemoveAppointmentGuest),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );

            ConfigIoC();
        }

        [Test]
        public void ConfirmAppointmentPushNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();

            var owner = new User
            {
                Id = 1
            };
            users.Add(owner);

            users.Add(new User
            {
                Id = 2,
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS"}
                }
            });

            appts.Add(new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id,
                GuestList = new List<AppointmentGuest>
                {
                    new AppointmentGuest {Id = 2, AppointmentId = 2, UserId = 2, Status = AppointmentGuestStatus.Confirmed},
                    new AppointmentGuest {Id = 3, AppointmentId = 2, UserId = 3, Status = AppointmentGuestStatus.Pending}
                }
            });

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<ConfirmAppointmentCommand, Appointment>, ConfirmAppointmentPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<ConfirmAppointmentCommand, Appointment>>();

            var request = new ConfirmAppointmentCommand { AppointmentId = 1 };
            var response = new Appointment { UserId = 1, User = new User { Name = "Justin" } };
            
            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "Workout Session Locked" && x.Ios.Payload.Type == NofiticationTypes.ConfirmAppointment),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );
            
            ConfigIoC();
        }

        [Test]
        public void ConfirmAppointmentGuestPushNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();
            var guest = Context.GetSet<AppointmentGuest>();

            var owner = new User
            {
                Id = 1
            };

            var user2 = new User
            {
                Id = 2,
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS"}
                }
            };

            var guest1 = new AppointmentGuest
            {
                Id = 2,
                AppointmentId = 2,
                UserId = 2,
                User = user2,
                Status = AppointmentGuestStatus.Confirmed
            };
            var guest2 = new AppointmentGuest
            {
                Id = 3,
                AppointmentId = 2,
                UserId = 3,
                Status = AppointmentGuestStatus.Pending
            };

            users.Add(owner);
            users.Add(user2);

            appts.Add(new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id,
                GuestList = new List<AppointmentGuest> {guest1, guest2}
            });

            guest.Add(guest1);
            guest.Add(guest2);

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<ConfirmAppointmentGuestCommand, AppointmentGuest>, ConfirmAppointmentGuestPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<ConfirmAppointmentGuestCommand, AppointmentGuest>>();

            var request = new ConfirmAppointmentGuestCommand { AppointmentId = 1, AppointmentGuestId = 2 };
            var response = new AppointmentGuest { Id = 1, UserId = 1, User = new User { Name = "Justin" } };
            
            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "Workout Session Confirmed" && x.Ios.Payload.Type == NofiticationTypes.ConfirmAppointmentGuest),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );
            
            ConfigIoC();
        }

        [Test]
        public void RequestFriendshipPushNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();

            var owner = new User
            {
                Id = 1,
                Name = "Owner"
            };

            var user2 = new User
            {
                Id = 2,
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS"}
                }
            };

            users.Add(owner);
            users.Add(user2);
            

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<RequestFriendshipCommand, Friendship>, RequestFriendshipPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<RequestFriendshipCommand, Friendship>>();

            var request = new RequestFriendshipCommand { UserId = owner.Id, FriendId = user2.Id };
            var response = new Friendship
            {
                Id = 1,
                UserId = owner.Id,
                FriendId = user2.Id,
                Status = FriendshipStatus.Pending,
                FriendshipKey = "1||2",
                Initiator = true
            };

            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Alert == "Owner wants to join the squad" && x.Ios.Payload.Type == NofiticationTypes.RequestFriendship),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );

            ConfigIoC();
        }

        [Test]
        public void ConfirmFriendshipPushNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();

            var owner = new User
            {
                Id = 1,
                Name = "Owner"
            };

            var user2 = new User
            {
                Id = 2,
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS"}
                }
            };

            users.Add(owner);
            users.Add(user2);


            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<ConfirmFriendshipCommand, Friendship>, ConfirmFriendshipPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<ConfirmFriendshipCommand, Friendship>>();

            var request = new ConfirmFriendshipCommand { UserId = owner.Id, FriendId = user2.Id };
            var response = new Friendship
            {
                Id = 1,
                UserId = owner.Id,
                FriendId = user2.Id,
                Status = FriendshipStatus.Active,
                FriendshipKey = "1||2",
                Initiator = false
            };

            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Alert == "Owner added you to the squad" && x.Ios.Payload.Type == NofiticationTypes.RequestFriendship),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );

            ConfigIoC();
        }

        [Test]
        public void DeleteAppointmentPushNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();

            var owner = new User
            {
                Id = 1
            };

            var guest1 = new User
            {
                Id = 2,
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS"}
                }
            };

            var guest2 = new User
            {
                Id = 3,
                Devices = new List<Device>
                {
                    new Device {PushToken = "654321", Platform = "Android"}
                }
            };

            users.Add(owner);
            users.Add(guest1);
            users.Add(guest2);

            appts.Add(new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id,
                GuestList = new List<AppointmentGuest>
                {
                    new AppointmentGuest {Id = 2, AppointmentId = 2, UserId = 2, Status = AppointmentGuestStatus.Confirmed},
                    new AppointmentGuest {Id = 3, AppointmentId = 2, UserId = 3, Status = AppointmentGuestStatus.Pending}
                }
            });

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<DeleteAppointmentCommand, Appointment>, DeleteAppointmentPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<DeleteAppointmentCommand, Appointment>>();

            var request = new DeleteAppointmentCommand
            {
                Id = 1,
                Guests = new List<User> {guest1, guest2},
                NotificaitonAlert = "Workout Session Canceled",
                NotificaitonTitle = "Workout Session Canceled"
            };

            var response = new Appointment { UserId = 1, User = new User { Name = "Justin" } };
           
            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Alert == "Workout Session Canceled" && x.Ios.Payload.Type == NofiticationTypes.CancelAppointment),
                Arg.Any<IEnumerable<Device>>()
                );
            
            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest1.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest2.Devices.First().PushToken))
                );
            
            ConfigIoC();
        }

        [Test]
        public void AppointmentOnMyWayPushNotifier_WhenInitiatedByGuest_CallsRestSharpMethodWithOwnerTokens()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();

            var owner = new User
            {
                Id = 1,
                Devices = new List<Device>
                {
                    new Device {PushToken = "000000", Platform = "iOS", UserId = 1}
                }
            };

            var guest1 = new User
            {
                Id = 2,
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS", UserId = 2}
                }
            };

            var guest2 = new User
            {
                Id = 3,
                Devices = new List<Device>
                {
                    new Device {PushToken = "654321", Platform = "Android", UserId = 3}
                }
            };

            users.Add(owner);
            users.Add(guest1);
            users.Add(guest2);

            var appt = new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id,
                GuestList = new List<AppointmentGuest>
                {
                    new AppointmentGuest
                    {
                        Id = 2,
                        AppointmentId = 1,
                        UserId = guest1.Id,
                        User = guest1,
                        Status = AppointmentGuestStatus.Confirmed
                    },
                    new AppointmentGuest
                    {
                        Id = 3,
                        AppointmentId = 1,
                        UserId = guest2.Id,
                        User = guest2,
                        Status = AppointmentGuestStatus.Pending
                    }
                }
            };

            appts.Add(appt);

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<AppointmentOnMyWayCommand, Appointment>, AppointmentOnMyWayPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AppointmentOnMyWayCommand, Appointment>>();

            var request = new AppointmentOnMyWayCommand
            {
                AppointmentId = appt.Id,
                UserId = guest1.Id
            };

            var response = new Appointment
            {
                GuestList = appt.GuestList,
                UserId = owner.Id,
                User = owner
            };

            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Alert == $"{guest1.Name} is on the way to the gym" && x.Ios.Payload.Type == NofiticationTypes.AddComment),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == owner.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).All(t => t != guest1.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).All(t => t != guest2.Devices.First().PushToken))
                );
            
            ConfigIoC();
        }

        [Test]
        public void AppointmentOnMyWayPushNotifier_WhenInitiatedByOwner_CallsRestSharpMethodWithGuestTokens()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();
            var apptGuests = Context.GetSet<AppointmentGuest>();

            var owner = new User
            {
                Id = 1,
                Name = "Owner",
                Devices = new List<Device>
                {
                    new Device {PushToken = "000000", Platform = "iOS", UserId = 1}
                }
            };

            var guest1 = new User
            {
                Id = 2,
                Name = "Guest 1",
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS", UserId = 2}
                }
            };

            var guest2 = new User
            {
                Id = 3,
                Devices = new List<Device>
                {
                    new Device {PushToken = "654321", Platform = "Android", UserId = 3}
                }
            };

            users.Add(owner);
            users.Add(guest1);
            users.Add(guest2);

            var apptGuest1 = new AppointmentGuest
            {
                Id = 2,
                AppointmentId = 1,
                UserId = guest1.Id,
                User = guest1,
                Status = AppointmentGuestStatus.Confirmed
            };

            var apptGuest2 = new AppointmentGuest
            {
                Id = 3,
                AppointmentId = 1,
                UserId = guest2.Id,
                User = guest2,
                Status = AppointmentGuestStatus.Pending
            };

            var appt = new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id,
                GuestList = new List<AppointmentGuest>
                {
                    apptGuest1,
                    apptGuest2
                }
            };

            appts.Add(appt);
            apptGuests.Add(apptGuest1);
            apptGuests.Add(apptGuest2);

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<AppointmentOnMyWayCommand, Appointment>, AppointmentOnMyWayPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AppointmentOnMyWayCommand, Appointment>>();

            var request = new AppointmentOnMyWayCommand
            {
                AppointmentId = appt.Id,
                UserId = owner.Id
            };

            var response = new Appointment
            {
                GuestList = appt.GuestList,
                UserId = owner.Id,
                User = owner
            };
            
            handler.Notify(request, response);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Alert == $"{owner.Name} is on the way to the gym" && x.Ios.Payload.Type == NofiticationTypes.AddComment),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).All(t => t != owner.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest1.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).All(t => t != guest2.Devices.First().PushToken))
                );

            ConfigIoC();
        }

        [Test]
        public void AppointmentAddCommentPushNotifier_WhenInitiatedByOwner_CallsRestSharpMethodWithGuestTokens()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();
            var apptGuests = Context.GetSet<AppointmentGuest>();

            var owner = new User
            {
                Id = 1,
                Name = "Owner",
                Devices = new List<Device>
                {
                    new Device {PushToken = "000000", Platform = "iOS", UserId = 1}
                }
            };

            var guest1 = new User
            {
                Id = 2,
                Name = "Guest 1",
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS", UserId = 2}
                }
            };

            var guest2 = new User
            {
                Id = 3,
                Devices = new List<Device>
                {
                    new Device {PushToken = "654321", Platform = "Android", UserId = 3}
                }
            };

            users.Add(owner);
            users.Add(guest1);
            users.Add(guest2);

            var apptGuest1 = new AppointmentGuest
            {
                Id = 2,
                AppointmentId = 1,
                UserId = guest1.Id,
                User = guest1,
                Status = AppointmentGuestStatus.Confirmed
            };

            var apptGuest2 = new AppointmentGuest
            {
                Id = 3,
                AppointmentId = 1,
                UserId = guest2.Id,
                User = guest2,
                Status = AppointmentGuestStatus.Pending
            };

            var appt = new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id,
                GuestList = new List<AppointmentGuest>
                {
                    apptGuest1,
                    apptGuest2
                }
            };

            appts.Add(appt);
            apptGuests.Add(apptGuest1);
            apptGuests.Add(apptGuest2);

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));
            Context.Register<IPostRequestHandler<AppointmentAddCommentCommand, Appointment>, AppointmentAddCommentPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AppointmentAddCommentCommand, Appointment>>();

            var request = new AppointmentAddCommentCommand
            {
                AppointmentId = appt.Id,
                UserId = owner.Id,
                Text = "Foo"
            };

            var response = new Appointment
            {
                GuestList = appt.GuestList,
                UserId = owner.Id,
                User = owner
            };
            
            handler.Notify(request, response);
            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Alert == $"[Comment] {owner.Name}: {request.Text}" && x.Ios.Payload.Type == NofiticationTypes.AddComment),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).All(t => t != owner.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest1.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest2.Devices.First().PushToken))
                );

            ConfigIoC();
        }

        [Test]
        public void AppointmentAddCommentPushNotifier_IncludesPending_CallsRestSharpMethodWithGuestTokens()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();
            var apptGuests = Context.GetSet<AppointmentGuest>();

            var owner = new User
            {
                Id = 1,
                Name = "Owner",
                Devices = new List<Device>
                {
                    new Device {PushToken = "000000", Platform = "iOS", UserId = 1}
                }
            };

            var guest1 = new User
            {
                Id = 2,
                Name = "Guest 1",
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS", UserId = 2}
                }
            };

            var guest2 = new User
            {
                Id = 3,
                Devices = new List<Device>
                {
                    new Device {PushToken = "654321", Platform = "Android", UserId = 3}
                }
            };

            users.Add(owner);
            users.Add(guest1);
            users.Add(guest2);

            var apptGuest1 = new AppointmentGuest
            {
                Id = 2,
                AppointmentId = 1,
                UserId = guest1.Id,
                User = guest1,
                Status = AppointmentGuestStatus.Confirmed
            };

            var apptGuest2 = new AppointmentGuest
            {
                Id = 3,
                AppointmentId = 1,
                UserId = guest2.Id,
                User = guest2,
                Status = AppointmentGuestStatus.Pending
            };

            var appt = new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id,
                GuestList = new List<AppointmentGuest>
                {
                    apptGuest1,
                    apptGuest2
                }
            };

            appts.Add(appt);
            apptGuests.Add(apptGuest1);
            apptGuests.Add(apptGuest2);

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));
            Context.Register<IPostRequestHandler<AppointmentAddCommentCommand, Appointment>, AppointmentAddCommentPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AppointmentAddCommentCommand, Appointment>>();

            var request = new AppointmentAddCommentCommand
            {
                AppointmentId = appt.Id,
                UserId = owner.Id,
                Text = "Foo"
            };

            var response = new Appointment
            {
                GuestList = appt.GuestList,
                UserId = owner.Id,
                User = owner
            };

            handler.Notify(request, response);
            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Alert == $"[Comment] {owner.Name}: {request.Text}" && x.Ios.Payload.Type == NofiticationTypes.AddComment),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).All(t => t != owner.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest1.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest2.Devices.First().PushToken))
                );

            ConfigIoC();
        }

        [Test]
        public void AppointmentAddCommentPushNotifier_IncludesCommentors_CallsRestSharpMethodWithGuestTokens()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();
            var apptGuests = Context.GetSet<AppointmentGuest>();
            var comments = Context.GetSet<AppointmentComment>();

            var owner = new User
            {
                Id = 1,
                Name = "Owner",
                Devices = new List<Device>
                {
                    new Device {PushToken = "000000", Platform = "iOS", UserId = 1}
                }
            };

            var guest1 = new User
            {
                Id = 2,
                Name = "Guest 1",
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS", UserId = 2}
                }
            };
            
            var guest2 = new User
            {
                Id = 3,
                Devices = new List<Device>
                {
                    new Device {PushToken = "654321", Platform = "Android", UserId = 3}
                }
            };

            var commentor = new User
            {
                Id = 99,
                Devices = new List<Device>
                {
                    new Device {PushToken = "99", Platform = "Android", UserId = 99}
                }
            };

            users.Add(owner);
            users.Add(guest1);
            users.Add(guest2);
            users.Add(commentor);

            comments.Add(new AppointmentComment
            {
                UserId = 99,
                User = commentor,
                AppointmentId = 1,
                Text = "Boom"
            });

            var apptGuest1 = new AppointmentGuest
            {
                Id = 2,
                AppointmentId = 1,
                UserId = guest1.Id,
                User = guest1,
                Status = AppointmentGuestStatus.Confirmed
            };

            var apptGuest2 = new AppointmentGuest
            {
                Id = 3,
                AppointmentId = 1,
                UserId = guest2.Id,
                User = guest2,
                Status = AppointmentGuestStatus.Pending
            };

            var appt = new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id,
                GuestList = new List<AppointmentGuest>
                {
                    apptGuest1,
                    apptGuest2
                }
            };

            appts.Add(appt);
            apptGuests.Add(apptGuest1);
            apptGuests.Add(apptGuest2);

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));
            Context.Register<IPostRequestHandler<AppointmentAddCommentCommand, Appointment>, AppointmentAddCommentPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AppointmentAddCommentCommand, Appointment>>();

            var request = new AppointmentAddCommentCommand
            {
                AppointmentId = appt.Id,
                UserId = owner.Id,
                Text = "Foo"
            };

            var response = new Appointment
            {
                GuestList = appt.GuestList,
                UserId = owner.Id,
                User = owner
            };

            handler.Notify(request, response);
            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Alert == $"[Comment] {owner.Name}: {request.Text}" && x.Ios.Payload.Type == NofiticationTypes.AddComment),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).All(t => t != owner.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest1.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest2.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == commentor.Devices.First().PushToken))
                );

            ConfigIoC();
        }


        [Test]
        public void AppointmentChangeTimesCommandPushNotifier_CallsRestSharpMethodWithGuestTokens()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();
            var apptGuests = Context.GetSet<AppointmentGuest>();

            var owner = new User
            {
                Id = 1,
                Name = "Owner",
                Devices = new List<Device>
                {
                    new Device {PushToken = "000000", Platform = "iOS", UserId = 1}
                }
            };

            var guest1 = new User
            {
                Id = 2,
                Name = "Guest 1",
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS", UserId = 2}
                }
            };

            var guest2 = new User
            {
                Id = 3,
                Devices = new List<Device>
                {
                    new Device {PushToken = "654321", Platform = "Android", UserId = 3}
                }
            };

            users.Add(owner);
            users.Add(guest1);
            users.Add(guest2);

            var apptGuest1 = new AppointmentGuest
            {
                Id = 2,
                AppointmentId = 1,
                UserId = guest1.Id,
                User = guest1,
                Status = AppointmentGuestStatus.Confirmed
            };

            var apptGuest2 = new AppointmentGuest
            {
                Id = 3,
                AppointmentId = 1,
                UserId = guest2.Id,
                User = guest2,
                Status = AppointmentGuestStatus.Pending
            };

            var appt = new Appointment
            {
                Id = 1,
                User = owner,
                UserId = owner.Id,
                GuestList = new List<AppointmentGuest>
                {
                    apptGuest1,
                    apptGuest2
                }
            };

            appts.Add(appt);
            apptGuests.Add(apptGuest1);
            apptGuests.Add(apptGuest2);

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));
            Context.Register<IPostRequestHandler<AppointmentChangeTimesCommand, Appointment>, AppointmentChangeTimesCommandPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AppointmentChangeTimesCommand, Appointment>>();

            var request = new AppointmentChangeTimesCommand
            {
                AppointmentId = appt.Id,
                UserId = owner.Id,
                TimeSlots = new List<DateTime?>()
            };

            var response = new Appointment
            {
                GuestList = appt.GuestList,
                UserId = owner.Id,
                User = owner
            };
            
            handler.Notify(request, response);
            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Alert == $"[Appointment] {owner.Name} changed the available times. Please review and join again." && x.Ios.Payload.Type == NofiticationTypes.AddComment),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).All(t => t != owner.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest1.Devices.First().PushToken))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == guest2.Devices.First().PushToken))
                );

            ConfigIoC();
        }

        public class FooPayload : AdditionalData
        {
            public string Foo { get; set; }
        }
    }
}