using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Generic;
using MediatR;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using WebGrease.Css.Extensions;

namespace justinobney.gymbuddy.api.DependencyResolution
{
    public class GenericCrudRequestsConvention : IRegistrationConvention
    {
        
        public void ScanTypes(TypeSet types, Registry registry)
        {

            types.FindTypes(TypeClassification.Concretes)
                .Where(
                    t =>
                        t.IsAbstract == false && typeof (IEntity).IsAssignableFrom(t)
                )
                .Distinct()
                .ForEach(
                    t =>
                    {
                        RegisterIDbSets(registry, t);
                        RegisterGetByIdQuery(registry, t);
                        RegisterGetByPredicateQuery(registry, t);
                    }
                );
        }

        private void RegisterIDbSets(Registry registry, Type type)
        {
            var dbSet = typeof(IDbSet<>).MakeGenericType(type);
            var method = typeof(ContainerExtensions).GetMethod("GetSet", BindingFlags.Static | BindingFlags.NonPublic);
            var genericMethod = method.MakeGenericMethod(type);
            
            registry.For(dbSet)
                .Use(ctx => genericMethod.Invoke(null, new object[] {ctx.GetInstance<AppContext>()}));
        }

        private static void RegisterGetByIdQuery(Registry registry, Type type)
        {
            var getByIdOfEntity = typeof (GetByIdQuery<>).MakeGenericType(type);
            var requestingType = typeof (IAsyncRequestHandler<,>).MakeGenericType(getByIdOfEntity, type);
            var concreteType = typeof (GetByIdQueryHandler<>).MakeGenericType(type);
            registry.AddType(requestingType, concreteType);
        }

        private void RegisterGetByPredicateQuery(Registry registry, Type type)
        {
            var queryableType = typeof(IQueryable<>).MakeGenericType(type);
            var getByPredicateQuery = typeof(GetAllByPredicateQuery<>).MakeGenericType(type);
            var requestingType = typeof(IRequestHandler<,>).MakeGenericType(getByPredicateQuery, queryableType);
            var concreteType = typeof(GetAllByPredicateQueryHandler<>).MakeGenericType(type);
            registry.AddType(requestingType, concreteType);
        }
    }
}