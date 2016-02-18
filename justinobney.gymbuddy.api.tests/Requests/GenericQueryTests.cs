using System;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Generic;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests
{
    [TestFixture]
    public class GenericQueryTests : BaseTest
    {
        [Test]
        public void GetAllByPredicate_ReturnsAllWhenNoPredicateSupplied()
        {
            Context.GetSet<User>().Attach(new User());
            Context.GetSet<User>().Attach(new User());
            Context.GetSet<User>().Attach(new User());

            var users = Mediator.Send(new GetAllByPredicateQuery<User>());

            users.Count().ShouldBe(3);
        }

        [Test]
        public void GetAllByPredicate_ReturnsEntitiesThatMatchPredicate()
        {
            Context.GetSet<User>().Attach(new User {Gender = Gender.Male});
            Context.GetSet<User>().Attach(new User {Gender = Gender.Female});
            Context.GetSet<User>().Attach(new User {Gender = Gender.Female});

            var request = new GetAllByPredicateQuery<User>(user => user.Gender == Gender.Male);
            var users = Mediator.Send(request);

            users.Count().ShouldBe(1);
        }

        [Test]
        public void GetByIdQuery_ReturnsCorrectEntity()
        {
            Context.GetSet<User>().Attach(new User { Id = 1 });
            Context.GetSet<User>().Attach(new User { Id = 2 });

            var request = new GetByIdQuery<User> {Id = 1};
            var user = Mediator.Send(request);

            user.Id.ShouldBe(1);
        }

        [Test]
        public void GetByIdQuery_ShouldThrowOnInvalidId()
        {
            Context.GetSet<User>().Attach(new User { Id = 1 });

            var request = new GetByIdQuery<User> { Id = 2 };
            Action execute = () => Mediator.Send(request);
            execute.ShouldThrow<NullReferenceException>("Entity does not exist");
        }
    }
}