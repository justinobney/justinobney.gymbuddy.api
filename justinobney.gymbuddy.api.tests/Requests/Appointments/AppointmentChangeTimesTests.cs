using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Appointments.Edit;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Appointments
{
    public class AppointmentChangeTimesTests : BaseTest
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
        public void AppointmentChangeTimesCommand_UpdatesAppointment()
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
            var timeslots = Context.GetSet<AppointmentTimeSlot>();

            appointments.Attach(appt);
            appointmentGuests.Attach(apptGuest);
            timeslots.Attach(timeslot);

            var newTimes = new List<DateTime?> { DateTime.UtcNow.AddHours(1) };

            var result = Mediator.Send(new AppointmentChangeTimesCommand
            {
                AppointmentId = 1,
                UserId = 2,
                TimeSlots = newTimes
            });

            result.TimeSlots.First().Time.ShouldBe(newTimes.First());
            timeslots.Count(x=>x.AppointmentId == 1).ShouldBe(0);
            appointmentGuests.Count(x=>x.AppointmentId == 1).ShouldBe(0);
        }

        [Test]
        public void AppointmentChangeTimesCommand_ThrowsValidationOnInvalidParams()
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

            Action execute = () => Mediator.Send(new AppointmentChangeTimesCommand
            {
                AppointmentId = 1,
                UserId = 2
            });

            execute.ShouldThrow<ValidationException>();
        }
    }
}