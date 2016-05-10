using System;
using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Guests;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Guests
{
    [TestFixture]
    public class GetOpenRequestsTests : BaseTest
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
        public void AddAppointmentGuestCommand_AddsGuestAndChangesStatus()
        {
            var appointments = Context.GetSet<Appointment>();
            var guests = Context.GetSet<AppointmentGuest>();
            var timeslots = Context.GetSet<AppointmentTimeSlot>();

            var timeslot = new AppointmentTimeSlot
            {
                Id = 2,
                AppointmentId = 1,
                Time = DateTime.UtcNow.AddHours(1)
            };

            var pendingGuest = new AppointmentGuest
            {
                Id = 3,
                AppointmentId = 1,
                UserId = 4,
                AppointmentTimeSlotId = 2,
                Status = AppointmentGuestStatus.Pending,
                TimeSlot = timeslot
            };

            var confirmedGuest = new AppointmentGuest
            {
                Id = 3,
                AppointmentId = 1,
                UserId = 4,
                AppointmentTimeSlotId = 2,
                Status = AppointmentGuestStatus.Confirmed,
                TimeSlot = timeslot
            };

            var appointment = new Appointment
            {
                Id = 1,
                UserId = CurrentUser.Id,
                GuestList = new List<AppointmentGuest>
                {
                    pendingGuest,
                    confirmedGuest
                },
                TimeSlots = new List<AppointmentTimeSlot>
                {
                    timeslot
                }
            };
            
            appointments.Attach(appointment);
            guests.Add(pendingGuest);
            timeslots.Add(timeslot);

            var requests = Mediator.Send(new GetOpenRequestsForUserQuery
            {
                UserId = CurrentUser.Id
            });

            requests.Count().ShouldBe(1);
        }
    }
}