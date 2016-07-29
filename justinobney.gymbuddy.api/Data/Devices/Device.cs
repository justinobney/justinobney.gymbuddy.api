using justinobney.gymbuddy.api.Interfaces;
using System;

namespace justinobney.gymbuddy.api.Data.Devices
{
    public class Device : IEntity, IHasTouchedProperties
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string DeviceId { get; set; }
        public string PushToken { get; set; }
        public string Platform { get; set; }
        public string ClientVersion { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}