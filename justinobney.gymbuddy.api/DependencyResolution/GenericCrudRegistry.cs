using justinobney.gymbuddy.api.Requests.Decorators;
using justinobney.gymbuddy.api.Requests.Generic;
using StructureMap;
using StructureMap.Graph;

namespace justinobney.gymbuddy.api.DependencyResolution
{
    public class GenericCrudRegistry : Registry
    {
        public GenericCrudRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType(typeof(GetByIdQuery<>));
                scan.Convention<GenericCrudRequestsConvention>();
            });
        }
    }
}