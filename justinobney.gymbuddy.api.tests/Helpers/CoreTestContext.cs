using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
        private readonly List<Type> _overriddenTypes = new List<Type>();

        public CoreTestContext(Container container)
        {
            _container = container;
        }

        public InMemoryDbSet<T> GetSet<T>() where T : class, IEntity
        {
            return (InMemoryDbSet<T>) _container.GetProfile("Test").GetInstance<IDbSet<T>>();
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

        public void Register<T, T2>() where T2 : T
        {
            var nestedContainer = _container.GetProfile("Test");
            nestedContainer.Configure(ctx =>
            {
                _overriddenTypes.Add(typeof(T));
                ctx.For<T>().Use<T2>();
            });
        }

        public void ResetIoC()
        {
            var nestedContainer = _container.GetProfile("Test");
            foreach (var type in _overriddenTypes)
            {
                var ejectMethod = nestedContainer.GetType().GetMethod("EjectAllInstancesOf", BindingFlags.Public | BindingFlags.Instance);
                var typedEjectMethod = ejectMethod.MakeGenericMethod(type);
                typedEjectMethod.Invoke(nestedContainer, new object[] {});
            }

            _overriddenTypes.Clear();
        }
    }
}