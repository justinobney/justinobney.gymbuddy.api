using System.Data.Entity;
using FakeDbSet;
using justinobney.gymbuddy.api.Data.Users;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Helpers
{
    [TestFixture]
    public class IoCTests : BaseTest
    {
        [Test]
        public void TestShouldAllowOverriddingIoCConfig()
        {
            var users = Context.GetSet<User>();
            users.GetType().ShouldBeSameAs(typeof(InMemoryDbSet<User>));
            users.GetType().ShouldNotBeSameAs(typeof(FakeInMemoryDbSet<User>));

            Context.Register<IDbSet<User>, FakeInMemoryDbSet<User>>();
            users = Context.GetSet<User>();
            users.GetType().ShouldBeSameAs(typeof(FakeInMemoryDbSet<User>));
            users.GetType().ShouldNotBeSameAs(typeof(InMemoryDbSet<User>));

            Context.ResetIoC();
            users = Context.GetSet<User>();
            users.GetType().ShouldBeSameAs(typeof(InMemoryDbSet<User>));
            users.GetType().ShouldNotBeSameAs(typeof(FakeInMemoryDbSet<User>));

            Context.Register<IDbSet<User>, FakeInMemoryDbSet<User>>();
            users = Context.GetSet<User>();
            users.GetType().ShouldBeSameAs(typeof(FakeInMemoryDbSet<User>));
            users.GetType().ShouldNotBeSameAs(typeof(InMemoryDbSet<User>));
        }

        public class FakeInMemoryDbSet<T> : InMemoryDbSet<T> where T : class
        {
            public FakeInMemoryDbSet()
            {
                
            }
            public FakeInMemoryDbSet(bool clearDownExistingData) : base(clearDownExistingData)
            {
            }
        }
    }
}