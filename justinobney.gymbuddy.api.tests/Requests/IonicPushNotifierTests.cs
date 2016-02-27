using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Requests.Appointments;
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

            var request = new ConfirmAppointmentCommand { AppointmentId = 1, AppointmentGuestIds = new List<long> {2} };
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
                    pushNotification.Notification.Title.ShouldBe("Workout Session Confirmed");
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


        public class FooPayload : AdditionalData
        {
            public string Foo { get; set; }
        }
    }
}