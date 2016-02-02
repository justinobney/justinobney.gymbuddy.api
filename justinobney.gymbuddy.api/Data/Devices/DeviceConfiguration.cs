using System.Data.Entity.ModelConfiguration;

namespace justinobney.gymbuddy.api.Data.Devices
{
    public class DeviceConfiguration : EntityTypeConfiguration<Device>
    {
        public DeviceConfiguration()
        {
            Property(x => x.DeviceId)
                .IsRequired();
        }
    }
}