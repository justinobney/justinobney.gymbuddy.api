using System;
using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Requests.Appointments.AddAppointmentGuest;
using justinobney.gymbuddy.api.Requests.Appointments.Comments;
using justinobney.gymbuddy.api.Requests.Appointments.Confirm;
using justinobney.gymbuddy.api.Requests.Appointments.Create;
using justinobney.gymbuddy.api.Requests.Appointments.Delete;
using justinobney.gymbuddy.api.Requests.Appointments.Edit;
using justinobney.gymbuddy.api.Requests.Appointments.RemoveAppointmentGuest;
using justinobney.gymbuddy.api.tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using NUnit.Framework;
using RestSharp;

namespace justinobney.gymbuddy.api.tests.Requests
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

            var expected = "{\"tokens\":[\"123\"],\"production\":false,\"notification\":{\"alert\":\"Alert\",\"title\":\"Title\",\"android\":{\"payload\":{\"foo\":\"Bar\",\"type\":\"Foo\"}},\"ios\":{\"payload\":{\"foo\":\"Bar\",\"type\":\"Foo\"},\"badge\":null}}}";
            JsonConvert.SerializeObject(notifier, serializationSettings).ShouldBe(expected);

            var expected2 = "{\"tokens\":null,\"production\":false,\"notification\":{\"alert\":null,\"title\":null,\"android\":{\"payload\":null},\"ios\":{\"payload\":null,\"badge\":null}}}";
            JsonConvert.SerializeObject(new IonicPushNotification(new NotificationPayload(null)), serializationSettings).ShouldBe(expected2);
        }

        [Test]
        public void CreateAppointmentNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();
            var gym = new Gym {Id = 1};
            users.Add(new User
            {
                Gyms = new List<Gym> { gym },
                Devices = new List<Device>
                {
                    new Device {PushToken = "123456", Platform = "iOS"}
                }
            });

            users.Add(new User
            {
                Gyms = new List<Gym> { gym },
                Devices = new List<Device>
                {
                    new Device {PushToken = "654321", Platform = "Android"}
                }
            });

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand {UserId = 1, GymId = 1};
            var response = new Appointment {UserId = 1, User = new User {Name = "Justin"} };
            var iosCalled = false;
            var androidCalled = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string) jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("New Appointment Available");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.CreateAppointment);
                    if (pushNotification.Tokens.Any(t => t == "123456"))
                    {
                        iosCalled = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == "654321"))
                    {
                        androidCalled = true;
                    }
                });
            
            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(2).Post(new RestRequest());
            iosCalled.ShouldBe(true);
            androidCalled.ShouldBe(true);
            ConfigIoC();
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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
            Context.Register<IPostRequestHandler<AddAppointmentGuestCommand, Appointment>, AddAppointmentGuestPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AddAppointmentGuestCommand, Appointment>>();

            var request = new AddAppointmentGuestCommand { AppointmentId = 1, UserId = 2 };
            var response = new Appointment { UserId = 1, User = new User { Name = "Justin" } };
            var iosCalled = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string)jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("Appointment Guest Request");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.AddAppointmentGuest);
                    if (pushNotification.Tokens.Any(t => t == "123456"))
                    {
                        iosCalled = true;
                    }
                });

            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(1).Post(new RestRequest());
            iosCalled.ShouldBe(true);
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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
            Context.Register<IPostRequestHandler<RemoveAppointmentGuestCommand, Appointment>, RemoveAppointmentGuestPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<RemoveAppointmentGuestCommand, Appointment>>();

            var request = new RemoveAppointmentGuestCommand { AppointmentId = 1, UserId = 2 };
            var response = new Appointment { UserId = 1, User = new User { Name = "Justin" } };
            var iosCalled = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string)jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("Appointment Guest Left :(");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.RemoveAppointmentGuest);
                    if (pushNotification.Tokens.Any(t => t == "123456"))
                    {
                        iosCalled = true;
                    }
                });

            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(1).Post(new RestRequest());
            iosCalled.ShouldBe(true);
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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
            Context.Register<IPostRequestHandler<ConfirmAppointmentCommand, Appointment>, ConfirmAppointmentPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<ConfirmAppointmentCommand, Appointment>>();

            var request = new ConfirmAppointmentCommand { AppointmentId = 1 };
            var response = new Appointment { UserId = 1, User = new User { Name = "Justin" } };
            var iosCalled = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string)jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("Workout Session Locked");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.ConfirmAppointment);
                    if (pushNotification.Tokens.Any(t => t == "123456"))
                    {
                        iosCalled = true;
                    }
                });

            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(1).Post(new RestRequest());
            iosCalled.ShouldBe(true);
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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
            Context.Register<IPostRequestHandler<ConfirmAppointmentGuestCommand, AppointmentGuest>, ConfirmAppointmentGuestPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<ConfirmAppointmentGuestCommand, AppointmentGuest>>();

            var request = new ConfirmAppointmentGuestCommand { AppointmentId = 1, AppointmentGuestId = 2 };
            var response = new AppointmentGuest { Id = 1, UserId = 1, User = new User { Name = "Justin" } };
            var iosCalled = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string)jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("Workout Session Confirmed");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.ConfirmAppointmentGuest);
                    if (pushNotification.Tokens.Any(t => t == "123456"))
                    {
                        iosCalled = true;
                    }
                });

            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(1).Post(new RestRequest());
            iosCalled.ShouldBe(true);
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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
            Context.Register<IPostRequestHandler<DeleteAppointmentCommand, Appointment>, DeleteAppointmentPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<DeleteAppointmentCommand, Appointment>>();

            var request = new DeleteAppointmentCommand
            {
                Id = 1,
                Guests = new List<User> {guest1, guest2},
                NotificaitonTitle = "Workout Session Canceled"
            };

            var response = new Appointment { UserId = 1, User = new User { Name = "Justin" } };
            var iosCalled = false;
            var androidCalled = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string)jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("Workout Session Canceled");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.CancelAppointment);
                    if (pushNotification.Tokens.Any(t => t == "123456"))
                    {
                        iosCalled = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == "654321"))
                    {
                        androidCalled = true;
                    }
                });

            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(2).Post(new RestRequest());
            iosCalled.ShouldBe(true);
            androidCalled.ShouldBe(true);
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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
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

            var ownerCalled = false;
            var guest1Called = false;
            var guest2Called = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string)jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("GymSquad");
                    pushNotification.Notification.Alert.ShouldBe($"{guest1.Name} is on the way to the gym");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.AddComment);
                    if (pushNotification.Tokens.Any(t => t == owner.Devices.First().PushToken))
                    {
                        ownerCalled = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == guest1.Devices.First().PushToken))
                    {
                        guest1Called = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == guest2.Devices.First().PushToken))
                    {
                        guest2Called = true;
                    }
                });

            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(1).Post(new RestRequest());
            ownerCalled.ShouldBe(true);
            guest1Called.ShouldBe(false);
            guest2Called.ShouldBe(false);
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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
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

            var ownerCalled = false;
            var guest1Called = false;
            var guest2Called = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string)jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("GymSquad");
                    pushNotification.Notification.Alert.ShouldBe($"{owner.Name} is on the way to the gym");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.AddComment);
                    if (pushNotification.Tokens.Any(t => t == owner.Devices.First().PushToken))
                    {
                        ownerCalled = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == guest1.Devices.First().PushToken))
                    {
                        guest1Called = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == guest2.Devices.First().PushToken))
                    {
                        guest2Called = true;
                    }
                });

            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(1).Post(new RestRequest());
            ownerCalled.ShouldBe(false);
            guest1Called.ShouldBe(true);
            guest2Called.ShouldBe(false);
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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
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

            var ownerCalled = false;
            var guest1Called = false;
            var guest2Called = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string)jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("GymSquad");
                    pushNotification.Notification.Alert.ShouldBe($"[Comment] {owner.Name}: {request.Text}");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.AddComment);
                    if (pushNotification.Tokens.Any(t => t == owner.Devices.First().PushToken))
                    {
                        ownerCalled = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == guest1.Devices.First().PushToken))
                    {
                        guest1Called = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == guest2.Devices.First().PushToken))
                    {
                        guest2Called = true;
                    }
                });

            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(1).Post(new RestRequest());
            ownerCalled.ShouldBe(false);
            guest1Called.ShouldBe(true);
            guest2Called.ShouldBe(false);
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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
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

            var ownerCalled = false;
            var guest1Called = false;
            var guest2Called = false;

            restClient
                .WhenForAnyArgs(client => client.Post(new RestRequest()))
                .Do(info =>
                {
                    var restRequest = info.Arg<RestRequest>();
                    var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                    var pushNotification =
                        JsonConvert.DeserializeObject<IonicPushNotification>((string)jsonPayload.Value);

                    restRequest.Resource.ShouldBe("/push");
                    restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                    pushNotification.Notification.Title.ShouldBe("GymSquad");
                    pushNotification.Notification.Alert.ShouldBe($"[Appointment] {owner.Name} changed the available times. You're request to join has been removed.");
                    pushNotification.Notification.Ios.Payload.Type.ShouldBe(NofiticationTypes.AddComment);
                    if (pushNotification.Tokens.Any(t => t == owner.Devices.First().PushToken))
                    {
                        ownerCalled = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == guest1.Devices.First().PushToken))
                    {
                        guest1Called = true;
                    }
                    if (pushNotification.Tokens.Any(t => t == guest2.Devices.First().PushToken))
                    {
                        guest2Called = true;
                    }
                });

            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(2).Post(new RestRequest());
            ownerCalled.ShouldBe(false);
            guest1Called.ShouldBe(true);
            guest2Called.ShouldBe(true);
            ConfigIoC();
        }

        public class FooPayload : AdditionalData
        {
            public string Foo { get; set; }
        }
    }
}