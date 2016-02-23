using System;
using System.Collections.Generic;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Appointments;
using NSubstitute;
using NUnit.Framework;
using StructureMap;

namespace justinobney.gymbuddy.api.tests.Requests
{
    [TestFixture]
    public class PostRequestHandlerTests : BaseTest
    {
        [Test]
        public void MediatorShouldNotifyAllRegisteredNotifiersForCommand()
        {
            var notifier = Substitute.For<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();
            Context.Container.Configure(container => container.AddRegistry(new FakeRegistry(notifier)));

            var request = new CreateAppointmentCommand
            {
                Id = 0,
                UserId = 1,
                GymId = 1,
                TimeSlots = new List<DateTime?> {DateTime.Now},
                Title = "Back Day"
            };
            var appt = Mediator.Send(request);

            notifier.Received(1).Notify(request,appt);
            SetUp();
        }

        public class FakeRegistry : Registry
        {
            public FakeRegistry(IPostRequestHandler<CreateAppointmentCommand, Appointment> notifier)
            {
                For<IPostRequestHandler<CreateAppointmentCommand, Appointment>>().Use(notifier);
            }
        }
    }
}