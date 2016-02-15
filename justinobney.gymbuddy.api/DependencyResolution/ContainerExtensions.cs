using System.Data.Entity;

namespace justinobney.gymbuddy.api.DependencyResolution
{
    public class ContainerExtensions
    {
        internal static IDbSet<T> GetSet<T>(DbContext context) where T : class
        {
            return context.Set<T>();
        }
    }
}