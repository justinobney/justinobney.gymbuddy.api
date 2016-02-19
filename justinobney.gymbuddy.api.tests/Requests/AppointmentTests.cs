using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Appointments;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests
{
    [TestFixture]
    public class AppointmentTests : BaseTest
    {
        [Test]
        public void CreateAppointmentCommand_ThrowsValidationException_OnMissingParameters()
        {
            Action foo = () => Mediator.Send(new CreateAppointmentCommand());
            foo.ShouldThrow<ValidationException>();
        }

        [Test]
        public void CreateAppointmentCommand_CreatesAppointment_WhenParamsValid()
        {
            var appt = Mediator.Send(new CreateAppointmentCommand
            {
                Id = 0,
                UserId = 1, // TODO: should throw on invalid user
                GymId = 1, // TODO: should throw on invalid gym
                TimeSlots = new List<DateTime?> {DateTime.Now},
                Title = "Back Day"
            });

            appt.TimeSlots.Count.ShouldBe(1);
            appt.Status.ShouldBe(AppointmentStatus.AwaitingGuests);
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
            appointments.Attach(new Appointment {Id = 1, GuestList = new List<AppointmentGuest>()});

            var appt = Mediator.Send(new AddAppointmentGuestCommand
            {
                AppointmentId = 1,
                UserId = 1, // todo: should throw on null
                AppointmentTimeSlotId = 1 // todo: should throw on null
            });

            appt.GuestList.Count.ShouldBe(1);
            appt.Status.ShouldBe(AppointmentStatus.PendingGuestConfirmation);
        }

        [Test]
        public void AddAppointmentGuestCommand_ThrowsOnDuplicateGuestEntriesPerTimeSlot()
        {
            var appointments = Context.GetSet<Appointment>();
            var appointmentGuests = Context.GetSet<AppointmentGuest>();
            appointments.Attach(new Appointment {Id = 1, GuestList = new List<AppointmentGuest>()});
            appointmentGuests.Attach(new AppointmentGuest {AppointmentId = 1, AppointmentTimeSlotId = 1, UserId = 1});
            
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

        [Test]
        public void GetAvailableAppointmentsForUserQuery_FiltersAppointments()
        {
            var gym = new Gym {Id = 1, Name = "GloboGym"};
            var currentUser = new User
            {
                Id = 1,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Brotege,
                FilterFitnessLevel = FitnessLevel.Tadpole,
                Gyms = new List<Gym> { gym }
            };

            Context.GetSet<User>().Attach(currentUser);
            Context.GetSet<Gym>().Attach(gym);
            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 1,
                GymId = gym.Id,
                UserId = currentUser.Id,
                TimeSlots = new List<AppointmentTimeSlot> {new AppointmentTimeSlot {Time = DateTime.Now.AddDays(1)}}
            });

            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 2,
                GymId = 2,
                UserId = 2,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.Now.AddDays(1) } }
            });

            var appts = Mediator.Send(new GetAvailableAppointmentsForUserQuery
            {
                UserId = currentUser.Id,
                GymIds = new List<long> {gym.Id}
            });

            appts.Count().ShouldBe(1);
        }
    }
}