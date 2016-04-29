using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Notifications;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Requests.Appointments.Create;
using justinobney.gymbuddy.api.tests.Helpers;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using RestSharp;

namespace justinobney.gymbuddy.api.tests.Notifiers
{
    [TestFixture]
    public class CreateAppointmentNotifierTests : BaseTest
    {
        [Test]
        public void CreateAppointmentNotifier_CallsRestSharpMethod()
        {
            var users = Context.GetSet<User>();
            var notifications = Context.GetSet<Notification>();

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

            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand { UserId = 1, GymId = 1 };
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

            notifications.Count(x => x.UserId == user1.Id).ShouldBe(1);
            iosCalled.ShouldBe(true);
            androidCalled.ShouldBe(true);

            ConfigIoC();
        }
    }
}