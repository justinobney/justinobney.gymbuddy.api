using System.Data.Entity;
using System.Linq;
using System.Reflection;
using FakeDbSet;
using justinobney.gymbuddy.api.Interfaces;
using StructureMap;

namespace justinobney.gymbuddy.api.tests.Helpers
{
    public class CoreTestContext
    {
        private readonly Container _container;

        public CoreTestContext(Container container)
        {
            _container = container;
        }

        public InMemoryDbSet<T> GetSet<T>() where T : class, IEntity
        {
            return (InMemoryDbSet<T>) _container.GetInstance<IDbSet<T>>();
        }

        public void ClearAll()
        {
            Assembly
                .GetAssembly(typeof(IEntity))
                .GetExportedTypes()
                .Where(t => t.IsAbstract == false && typeof(IEntity).IsAssignableFrom(t))
                .Distinct()
                .ToList()
                .ForEach(
                    t =>
                    {
                        var inMemorySetType = typeof(InMemoryDbSet<>).MakeGenericType(t);
                        var method = typeof(CoreTestContext).GetMethod("GetSet", BindingFlags.Public | BindingFlags.Instance);
                        var genericMethod = method.MakeGenericMethod(t);
                        var inMemorySet = genericMethod.Invoke(this, new object[] { });
                        var clearMethod = inMemorySetType.GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance);
                        clearMethod.Invoke(inMemorySet, new object[] { });
                    }
                );
        }
    }
}