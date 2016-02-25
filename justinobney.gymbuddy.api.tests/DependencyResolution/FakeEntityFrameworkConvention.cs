using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Interfaces;
using NSubstitute;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace justinobney.gymbuddy.api.tests.DependencyResolution
{
    public class FakeEntityFrameworkConvention : IRegistrationConvention
    {

        public void ScanTypes(TypeSet types, Registry registry)
        {

            types.FindTypes(TypeClassification.Concretes)
                .Where(
                    t =>
                        t.IsAbstract == false && typeof(IEntity).IsAssignableFrom(t)
                )
                .Distinct()
                .ToList()
                .ForEach(
                    t =>
                    {
                        RegisterIDbSets(registry, t);
                    }
                );

            var context = Substitute.For<AppContext>();
            registry.For<AppContext>().Use(context);
        }

        private void RegisterIDbSets(Registry registry, Type type)
        {
            var dbSet = typeof(IDbSet<>).MakeGenericType(type);
            var method = typeof(TestContainerExtensions).GetMethod("GetSet", BindingFlags.Static | BindingFlags.NonPublic);
            var genericMethod = method.MakeGenericMethod(type);

            registry.For(dbSet)
                .Use(ctx => genericMethod.Invoke(null, new object[] { }));
        }
        
    }
}