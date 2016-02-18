using justinobney.gymbuddy.api.Data.Users;
using StructureMap;
using StructureMap.Graph;

namespace justinobney.gymbuddy.api.tests.DependencyResolution
{
    public class FakeEntityFrameworkRegistry : Registry
    {
        public FakeEntityFrameworkRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType(typeof(User));
                scan.Convention<FakeEntityFrameworkConvention>();
                scan.WithDefaultConventions();
            });
        }
    }
}