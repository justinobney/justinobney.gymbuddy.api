using StructureMap;

namespace justinobney.gymbuddy.api.DependencyResolution
{
    public class EntityFrameworkRegistry : Registry
    {
        public EntityFrameworkRegistry()
        {
            Scan(scan => scan.Convention<EntityFrameworkConvention>());
        }
    }
}