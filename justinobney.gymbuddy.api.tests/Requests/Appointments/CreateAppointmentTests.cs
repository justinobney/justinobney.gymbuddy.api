using System;
using System.Collections.Generic;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Appointments.Create;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests.Appointments
{
    [TestFixture]
    public class CreateAppointmentTests : BaseTest
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
                UserId = 1, // TODO: should throw on invalid user
                GymId = 1, // TODO: should throw on invalid gym
                TimeSlots = new List<DateTime?> {DateTime.Now},
                Title = "Back Day"
            });

            appt.TimeSlots.Count.ShouldBe(1);
            appt.Status.ShouldBe(AppointmentStatus.AwaitingGuests);
        }
    }
}