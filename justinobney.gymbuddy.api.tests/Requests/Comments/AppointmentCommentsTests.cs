using System.Collections.Generic;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Appointments.Comments;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Comments
{
    [TestFixture]
    public class AppointmentCommentsTests : BaseTest
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
        public void AppointmentOnMyWayCommand_AddsCommentToAppointmentComments()
        {
            var appts = Context.GetSet<Appointment>();
            var comments = Context.GetSet<AppointmentComment>();

            appts.Add(new Appointment
            {
                Id = 1,
                User = CurrentUser,
                GuestList = new List<AppointmentGuest> { new AppointmentGuest { User = new User { Id = 1 } } }
            });

            var request = new AppointmentOnMyWayCommand
            {
                AppointmentId = 1,
                UserId = CurrentUser.Id
            };
            Mediator.Send(request);

            appts.Count().ShouldBe(1);
            comments.Count().ShouldBe(1);
            comments.First().Text.ShouldBe("User is on the way to the gym");
        }
    }
}