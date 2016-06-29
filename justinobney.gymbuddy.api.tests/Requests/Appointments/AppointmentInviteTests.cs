using System;
using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Requests.Appointments;
using NSubstitute;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Appointments
{
    public class AppointmentInviteTests : BaseTest
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
            var apptGuest1 = new AppointmentGuest
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
                GuestList = new List<AppointmentGuest> { apptGuest1 },
                TimeSlots = new List<AppointmentTimeSlot> { timeslot }
            };

            var friend = new User
            {
                Id = 3,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Gyms = new List<Gym> {DefaultGym},
                Name = "User",
                Devices = new List<Device>
                {
                    new Device { Platform = "iOS", PushToken = "123456"}
                }
            };

            var appointments = Context.GetSet<Appointment>();
            var appointmentGuests = Context.GetSet<AppointmentGuest>();
            var timeslots = Context.GetSet<AppointmentTimeSlot>();
            var users = Context.GetSet<User>();

            appointments.Attach(appt);
            appointmentGuests.Attach(apptGuest1);
            timeslots.Attach(timeslot);
            users.Attach(friend);
            
            Mediator.Send(new AppointmentInviteFriendCommand
            {
                AppointmentId = appt.Id,
                UserId = CurrentUser.Id,
                FriendId = friend.Id
            });

            var notifier = Context.GetInstance<IPushNotifier>();

            notifier.Received().Send(
                Arg.Is<NotificationPayload>(x => x.Message == "Gainz! User has invited you to workout" && x.Ios.Payload.Type == NofiticationTypes.CreateAppointment),
                Arg.Any<IEnumerable<Device>>()
                );

            notifier.Received().Send(
                Arg.Any<NotificationPayload>(),
                Arg.Is<IEnumerable<Device>>(x => x.Select(y => y.PushToken).Any(t => t == "123456"))
                );
        }
    }
}