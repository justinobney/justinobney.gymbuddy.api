using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
using StructureMap;
using StructureMap.Graph;

namespace justinobney.gymbuddy.api.DependencyResolution.Registries
{
    public class NotificationRegistry : Registry
    {
        public NotificationRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.AssemblyContainingType(typeof(IPostRequestHandler<,>));
                    
                    scan.AddAllTypesOf(typeof(IPostRequestHandler<,>));
                    var handlerType = For(typeof(IRequestHandler<,>));
                    handlerType.DecorateAllWith(typeof(PostRequestHandler<,>));
                });
        }
    }
}