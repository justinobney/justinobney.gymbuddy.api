using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Appointments.Delete;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Appointments
{
    [TestFixture]
    public class DeleteAppointmentTests : BaseTest
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
        public void DeleteAppointmentCommand_RemovesAppointment()
        {
            var appts = Context.GetSet<Appointment>();
            appts.Add(new Appointment
            {
                Id = 1,
                User = CurrentUser,
                GuestList = new List<AppointmentGuest> { new AppointmentGuest { User = new User { Id = 1 } } }
            });

            var request = new DeleteAppointmentCommand { Id = 1 };
            Mediator.Send(request);

            request.NotificaitonTitle.ShouldBe("Workout Session Canceled");
            request.NotificaitonAlert.ShouldBe("User canceled");
            request.Guests.Count().ShouldBe(1);
            appts.Count().ShouldBe(0);
        }
    }
}