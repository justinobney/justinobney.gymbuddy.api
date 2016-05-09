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
using NSubstitute;
using NUnit.Framework;

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
            
            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));
            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand { UserId = 1, GymId = 1 };
            var response = new Appointment { UserId = 1, User = new User { Name = "Justin" } };
            
            handler.Notify(request, response);
            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Title == "New Appointment Available"),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x=>x.Select(y=>y.PushToken).Any(t => t == "123456"))
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "654321"))
                );
            
            ConfigIoC();
        }
    }
}