using System;
using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Appointments;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Appointments
{
    [TestFixture]
    public class AppointmentQueryTests : BaseTest
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
        public void GetAvailableAppointmentsForUserQuery_FiltersAppointments()
        {
            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 1,
                GymId = DefaultGym.Id,
                UserId = CurrentUser.Id,
                User = CurrentUser,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.UtcNow.AddMinutes(-29) } }
            });

            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 1,
                GymId = DefaultGym.Id,
                UserId = CurrentUser.Id,
                User = CurrentUser,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.UtcNow.AddMinutes(-30) } }
            });

            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 1,
                GymId = DefaultGym.Id,
                UserId = 2,
                User = new User { Id = 2, FilterGender = GenderFilter.Female, Gender = Gender.Female },
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.UtcNow.AddMinutes(-29) } }
            });

            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 1,
                GymId = DefaultGym.Id,
                UserId = CurrentUser.Id,
                User = CurrentUser,
                Status = AppointmentStatus.Confirmed,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.UtcNow.AddMinutes(-29) } }
            });

            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 2,
                GymId = 2,
                UserId = 2,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.UtcNow.AddMinutes(-29) } }
            });

            var appts = Mediator.Send(new GetAvailableAppointmentsForUserQuery
            {
                UserId = CurrentUser.Id,
                GymIds = new List<long> { DefaultGym.Id }
            });

            appts.Count().ShouldBe(1);
        }

        [Test]
        public void GetScheduledAppointmentsForUserQuery_FiltersAppointments()
        {
            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 1,
                GymId = DefaultGym.Id,
                GuestList = new List<AppointmentGuest>(),
                UserId = CurrentUser.Id,
                Status = AppointmentStatus.PendingGuestConfirmation,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.Now.AddDays(1) } }
            });

            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 2,
                GymId = 2,
                GuestList = new List<AppointmentGuest>(),
                UserId = 2,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.Now.AddDays(1) } }
            });

            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 3,
                GymId = DefaultGym.Id,
                GuestList = new List<AppointmentGuest>(),
                UserId = 2,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.Now.AddDays(1) } }
            });

            var timeslot = new AppointmentTimeSlot { Id = 1, Time = DateTime.Now.AddDays(1) };
            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 4,
                GymId = DefaultGym.Id,
                UserId = 2,
                TimeSlots = new List<AppointmentTimeSlot> { timeslot },
                Status = AppointmentStatus.PendingGuestConfirmation,
                GuestList = new List<AppointmentGuest> { new AppointmentGuest { UserId = CurrentUser.Id, AppointmentTimeSlotId = timeslot.Id, AppointmentId = 4, Status = AppointmentGuestStatus.Pending } }
            });

            var appts = Mediator.Send(new GetScheduledAppointmentsForUserQuery
            {
                UserId = CurrentUser.Id
            });

            appts.Count().ShouldBe(2);
        }

        [Test]
        public void GetScheduledAppointmentsForUserQuery_HandlesPendingStatus()
        {
            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 1,
                GymId = DefaultGym.Id,
                GuestList = new List<AppointmentGuest>(),
                UserId = CurrentUser.Id,
                Status = AppointmentStatus.Confirmed,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.UtcNow.AddMinutes(-29) } }
            });

            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 2,
                GymId = 2,
                GuestList = new List<AppointmentGuest>(),
                UserId = 2,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.UtcNow.AddMinutes(-29) } }
            });

            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 3,
                GymId = DefaultGym.Id,
                GuestList = new List<AppointmentGuest>(),
                UserId = 2,
                TimeSlots = new List<AppointmentTimeSlot> { new AppointmentTimeSlot { Time = DateTime.UtcNow.AddMinutes(-29) } }
            });

            var timeslot = new AppointmentTimeSlot { Id = 1, Time = DateTime.UtcNow.AddMinutes(-29) };
            Context.GetSet<Appointment>().Attach(new Appointment
            {
                Id = 4,
                GymId = DefaultGym.Id,
                UserId = 2,
                TimeSlots = new List<AppointmentTimeSlot> { timeslot },
                Status = AppointmentStatus.Confirmed,
                GuestList = new List<AppointmentGuest> { new AppointmentGuest { UserId = CurrentUser.Id, AppointmentTimeSlotId = timeslot.Id, AppointmentId = 4, Status = AppointmentGuestStatus.Pending } }
            });

            var appts = Mediator.Send(new GetScheduledAppointmentsForUserQuery
            {
                UserId = CurrentUser.Id
            });

            appts.Count().ShouldBe(1);
        }
    }
}