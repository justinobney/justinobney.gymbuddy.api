using System;
using System.Linq;
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
                        RegisterGetByIdQuery(registry, t);
                        RegisterGetByPredicateQuery(registry, t);
                    }
                );
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