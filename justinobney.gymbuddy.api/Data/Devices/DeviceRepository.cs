namespace justinobney.gymbuddy.api.Data.Devices
{
    public class DeviceRepository : BaseRepository<Device>
    {
        public DeviceRepository(AppContext context) : base(context)
        {
        }
    }
}