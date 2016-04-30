using System.Collections.Generic;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IPushNotifier
    {
        void Send(NotificationPayload message, IEnumerable<Device> devices);
    }
}