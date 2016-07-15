using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Gyms;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace justinobney.gymbuddy.api.tests.Requests.Gyms
{
    [TestFixture]
    public class GymTests : BaseTest
    {

        [SetUp]
        public void Setup()
        {
            var gym1 = new Gym { Id = 1, Name = "GloboGym" };
            var gym2 = new Gym { Id = 2, Name = "Average Joes" };
            var gym3 = new Gym { Id = 3, Name = "Tony Perkis System at Camp Hope" };
            var user1 = new User
            {
                Id = 1,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Gyms = new List<Gym> { gym1 },
                Name = "User1",
                FacebookUserId = "1122334455"
            };
            var user2 = new User
            {
                Id = 2,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Gyms = new List<Gym>(),
                Name = "User2",
                FacebookUserId = "192837"
            };
            var user3 = new User
            {
                Id = 3,
                Gender = Gender.Male,
                FilterGender = GenderFilter.Both,
                FitnessLevel = FitnessLevel.Intermediate,
                FilterFitnessLevel = FitnessLevel.Beginner,
                Gyms = new List<Gym> { gym2 },
                Name = "User3",
                FacebookUserId = "3333456"
            };



            Context.GetSet<User>().Attach(user1);
            Context.GetSet<User>().Attach(user2);
            Context.GetSet<User>().Attach(user3);
            Context.GetSet<Gym>().Attach(gym1);
            Context.GetSet<Gym>().Attach(gym2);
            Context.GetSet<Gym>().Attach(gym3);
        }

        [Test]
        public void GetGymsCommandUserHasNotJoinedGym()
        {
            var command = new GetGymsCommand
            {
                UserId = 1
            };

            var response = Mediator.Send(command);
            response.Count(x => x.Id == 3 && x.HasUserJoinedGym).ShouldBe(0);
        }

        [Test]
        public void GetGymsCommandUserHasJoinedGym()
        {
            var command = new GetGymsCommand
            {
                UserId = 1
            };

            var response = Mediator.Send(command);
            response.Count(x => x.Id == 1 && x.HasUserJoinedGym).ShouldBe(1);
        }

        [Test]
        public void GetGymsCommandUserHasJoinedGymWhenUserHasNoGyms()
        {
            var command = new GetGymsCommand
            {
                UserId = 2
            };

            var response = Mediator.Send(command);
            response.Count(x => !x.HasUserJoinedGym).ShouldBe(3);
        }
    }
}
