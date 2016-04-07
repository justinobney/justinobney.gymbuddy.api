using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Appointments.Confirm;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Appointments
{
    [TestFixture]
    public class ConfirmAppointmentTests : BaseTest
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
        public void ConfirmAppointmentCommand_UpdatesAppointment()
        {
            var timeslot = new AppointmentTimeSlot { Id = 1, AppointmentId = 1, Time = DateTime.Now };
            var apptGuest = new AppointmentGuest
            {
                AppointmentId = 1,
                AppointmentTimeSlotId = 1,
                UserId = 2,
                TimeSlot = timeslot,
                Status = AppointmentGuestStatus.Confirmed
            };
            var appt = new Appointment
            {
                Id = 1,
                UserId = 1,
                GuestList = new List<AppointmentGuest> { apptGuest },
                TimeSlots = new List<AppointmentTimeSlot> { timeslot }
            };
            var appointments = Context.GetSet<Appointment>();
            var appointmentGuests = Context.GetSet<AppointmentGuest>();
            appointments.Attach(appt);
            appointmentGuests.Attach(apptGuest);

            var result = Mediator.Send(new ConfirmAppointmentCommand
            {
                AppointmentId = appt.Id
            });

            result.ConfirmedTime.ShouldBe(timeslot.Time);
            result.GuestList.First().Status.ShouldBe(AppointmentGuestStatus.Confirmed);
            result.Status.ShouldBe(AppointmentStatus.Confirmed);
        }

        [Test]
        public void ConfirmAppointmentGuestCommand_UpdatesAppointment()
        {
            var timeslot = new AppointmentTimeSlot { Id = 1, AppointmentId = 1, Time = DateTime.Now };
            var apptGuest = new AppointmentGuest
            {
                Id = 1,
                AppointmentId = 1,
                AppointmentTimeSlotId = 1,
                UserId = 2,
                TimeSlot = timeslot,
                Status = AppointmentGuestStatus.Confirmed
            };
            var appt = new Appointment
            {
                Id = 1,
                UserId = 1,
                GuestList = new List<AppointmentGuest> { apptGuest },
                TimeSlots = new List<AppointmentTimeSlot> { timeslot }
            };
            var appointments = Context.GetSet<Appointment>();
            var appointmentGuests = Context.GetSet<AppointmentGuest>();
            appointments.Attach(appt);
            appointmentGuests.Attach(apptGuest);

            var result = Mediator.Send(new ConfirmAppointmentGuestCommand
            {
                AppointmentId = 1,
                AppointmentGuestId = 1
            });

            result.Status.ShouldBe(AppointmentGuestStatus.Confirmed);
        }

        [Test]
        public void ConfirmAppointmentCommand_ThrowsValidationOnInvalidParams()
        {
            Action execute = () => Mediator.Send(new ConfirmAppointmentCommand());
            execute.ShouldThrow<ValidationException>();
        }
    }
}