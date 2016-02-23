using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
using StructureMap;
using StructureMap.Graph;

namespace justinobney.gymbuddy.api.tests.DependencyResolution
{
    public class FakeNotificationRegistry : Registry
    {
        public FakeNotificationRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                var handlerType = For(typeof(IRequestHandler<,>));
                handlerType.DecorateAllWith(typeof(PostRequestHandler<,>));
            });
        }
    }
}