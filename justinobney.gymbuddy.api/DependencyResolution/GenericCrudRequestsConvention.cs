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
                        var getByIdOfEntity = typeof(GetByIdQuery<>).MakeGenericType(t);
                        var requestingType = typeof(IAsyncRequestHandler<,>).MakeGenericType(getByIdOfEntity, t);
                        var concreteType = typeof(GetByIdQueryHandler<>).MakeGenericType(t);
                        registry.AddType(requestingType, concreteType);
                    }
                );
        }
    }
}