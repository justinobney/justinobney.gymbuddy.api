using StructureMap;

namespace justinobney.gymbuddy.api.DependencyResolution
{
    public class GenericCrudRegistry : Registry
    {
        public GenericCrudRegistry()
        {
            Scan(scan => scan.Convention<GenericCrudRequestsConvention>());
        }
    }
}