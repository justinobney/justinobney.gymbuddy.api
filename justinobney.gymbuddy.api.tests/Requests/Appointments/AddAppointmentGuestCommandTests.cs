using System;
using FluentValidation;
using justinobney.gymbuddy.api.Requests.Appointments;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Appointments
{
    public class AddAppointmentGuestCommandTests : BaseTest
    {
        [Test]
        public void ThrowsValidationException_WhenAppointmentDoesNotExist()
        {
            Action foo = () => Mediator.Send(new AddAppointmentGuestCommand());
            foo.ShouldThrow<ValidationException>();
        }
    }
}