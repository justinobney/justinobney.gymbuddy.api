using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Generic;
using StructureMap;
using StructureMap.Graph;

namespace justinobney.gymbuddy.api.DependencyResolution
{
    public class EntityFrameworkRegistry : Registry
    {
        public EntityFrameworkRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType(typeof(IEntity));
                scan.Convention<EntityFrameworkConvention>();
            });
        }
    }
}