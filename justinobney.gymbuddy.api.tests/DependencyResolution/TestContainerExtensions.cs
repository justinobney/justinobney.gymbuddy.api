using System.Data.Entity;

namespace justinobney.gymbuddy.api.tests.DependencyResolution
{
    public class TestContainerExtensions
    {
        internal static IDbSet<T> GetSet<T>() where T : class
        {
            return new FakeDbSet.InMemoryDbSet<T>();
        }
    }
}