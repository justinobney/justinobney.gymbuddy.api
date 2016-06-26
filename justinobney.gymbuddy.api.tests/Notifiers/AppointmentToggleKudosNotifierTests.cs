using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Requests.Appointments.Kudos;
using NSubstitute;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Notifiers
{
    [TestFixture]
    public class AppointmentToggleKudosNotifierTests : BaseTest
    {
        public User Owner = new User
        {
            Id = 1,
            Name = "Justin"
        };

        public User Guest = new User
        {
            Id = 2,
            Devices = new List<Device> { new Device {PushToken = "123456", Platform = "iOS"} }
        };

        [Test]
        public void AppointmentToggleKudosNotifier_SendsNotificationOnToggleOn()
        {
            var notifier = Context.Container.GetInstance<IPushNotifier>();

            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();
            var kudos = Context.GetSet<AppointmentKudos>();

            var appt = new Appointment
            {
                Id = 1,
                User = Guest,
                UserId = Guest.Id
            };

            var theKudos = new AppointmentKudos
            {
                AppointmentId = appt.Id,
                UserId = Owner.Id
            };

            users.Add(Owner);
            users.Add(Guest);
            appts.Add(appt);
            kudos.Add(theKudos);
            
            Context.Register<IPostRequestHandler<AppointmentToggleKudosCommand, Appointment>, AppointmentToggleKudosPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AppointmentToggleKudosCommand, Appointment>>();

            var request = new AppointmentToggleKudosCommand
            {
                AppointmentId = appt.Id,
                UserId = Owner.Id
            };
            
            handler.Notify(request, appt);

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x =>
                    x.Message == $"Kudos: {Owner.Name} approves"
                    && x.Ios.Payload.Type == NofiticationTypes.AppointmentKudos),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );

            ConfigIoC();
        }

        [Test]
        public void AppointmentToggleKudosNotifier_DoesNotSendsNotificationOnToggleOff()
        {
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();
            

            var appt = new Appointment
            {
                Id = 1,
                User = Guest,
                UserId = Guest.Id
            };
            
            users.Add(Owner);
            users.Add(Guest);
            appts.Add(appt);

            var notifier = Substitute.For<IPushNotifier>();
            Context.Container.Configure(container => container.For<IPushNotifier>().Use(notifier));

            Context.Register<IPostRequestHandler<AppointmentToggleKudosCommand, Appointment>, AppointmentToggleKudosPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AppointmentToggleKudosCommand, Appointment>>();

            var request = new AppointmentToggleKudosCommand
            {
                AppointmentId = appt.Id,
                UserId = Owner.Id
            };

            handler.Notify(request, appt);

            notifier.DidNotReceive().Send(
                Arg.Is<NotificationPayload>(x =>
                    x.Message == $"Kudos: {Owner.Name} approves"
                    && x.Ios.Payload.Type == NofiticationTypes.AppointmentKudos),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.DidNotReceive().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );

            ConfigIoC();
        }

        [Test]
        public void AppointmentToggleKudosNotifier_DoesNotNotifySelfKudos()
        {
            var notifier = Context.Container.GetInstance<IPushNotifier>();
            var users = Context.GetSet<User>();
            var appts = Context.GetSet<Appointment>();

            var appt = new Appointment
            {
                Id = 1,
                User = Guest,
                UserId = Guest.Id
            };
            
            users.Add(Owner);
            users.Add(Guest);
            appts.Add(appt);
            
            Context.Register<IPostRequestHandler<AppointmentToggleKudosCommand, Appointment>, AppointmentToggleKudosPushNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<AppointmentToggleKudosCommand, Appointment>>();

            var request = new AppointmentToggleKudosCommand
            {
                AppointmentId = appt.Id,
                UserId = Guest.Id
            };

            handler.Notify(request, appt);

            notifier.DidNotReceive().Send(
                Arg.Is<NotificationPayload>(x =>
                    x.Message == $"Kudos: {Owner.Name} approves"
                    && x.Ios.Payload.Type == NofiticationTypes.AppointmentKudos),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.DidNotReceive().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );

            ConfigIoC();
        }
    }
}