using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Appointments;
using justinobney.gymbuddy.api.Requests.Appointments.AddAppointmentGuest;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Appointments
{
    [TestFixture]
    public class AddAppointmentGuestTests : BaseTest
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
                Name = "User"
            };
            Context.GetSet<User>().Attach(CurrentUser);
            Context.GetSet<Gym>().Attach(DefaultGym);
        }

        [Test]
        public void AddAppointmentGuestCommand_ThrowsValidationException_WhenAppointmentDoesNotExist()
        {
            Action foo = () => Mediator.Send(new AddAppointmentGuestCommand());
            foo.ShouldThrow<ValidationException>();
        }

        [Test]
        public void AddAppointmentGuestCommand_AddsGuestAndChangesStatus()
        {
            var appointments = Context.GetSet<Appointment>();
            var guests = Context.GetSet<AppointmentGuest>();
            appointments.Attach(new Appointment { Id = 1, GuestList = new List<AppointmentGuest>() });

            var guest = Mediator.Send(new AddAppointmentGuestCommand
            {
                AppointmentId = 1,
                UserId = 1, // todo: should throw on null
                AppointmentTimeSlotId = 1 // todo: should throw on null
            });

            guests.Select(x=>x.AppointmentTimeSlotId == guest.AppointmentTimeSlotId).Count().ShouldBe(1);
            var appt = appointments.First(x => x.Id == guest.AppointmentId);
            appt.Status.ShouldBe(AppointmentStatus.PendingGuestConfirmation);
        }

        [Test]
        public void AddAppointmentGuestCommand_ThrowsOnDuplicateGuestEntriesPerTimeSlot()
        {
            var appointments = Context.GetSet<Appointment>();
            var appointmentGuests = Context.GetSet<AppointmentGuest>();
            appointments.Attach(new Appointment { Id = 1, GuestList = new List<AppointmentGuest>() });
            appointmentGuests.Attach(new AppointmentGuest { AppointmentId = 1, AppointmentTimeSlotId = 1, UserId = 1 });

            Action invalidCommand = () => Mediator.Send(new AddAppointmentGuestCommand
            {
                AppointmentId = 1,
                UserId = 1,
                AppointmentTimeSlotId = 1
            });

            invalidCommand.ShouldThrow<ValidationException>(@"Validation failed: 
 -- This user is already registered for this time slot");
        }

        [Test]
        public void AddAppointmentGuestCommand_ThrowsWhenOwnerRegistersAsGuest()
        {
            var appointments = Context.GetSet<Appointment>();
            var appointmentGuests = Context.GetSet<AppointmentGuest>();
            appointments.Attach(new Appointment { Id = 1, UserId = 1, GuestList = new List<AppointmentGuest>() });
            appointmentGuests.Attach(new AppointmentGuest { AppointmentId = 1, AppointmentTimeSlotId = 1, UserId = 2 });

            Action invalidCommand = () => Mediator.Send(new AddAppointmentGuestCommand
            {
                AppointmentId = 1,
                UserId = 1,
                AppointmentTimeSlotId = 1
            });

            invalidCommand.ShouldThrow<ValidationException>(@"Validation failed: 
 -- You can not be your own guest");
        }
    }
}