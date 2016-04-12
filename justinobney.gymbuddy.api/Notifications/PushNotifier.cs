using System.Linq;
using justinobney.gymbuddy.api.Data.Devices;
using RestSharp;

namespace justinobney.gymbuddy.api.Notifications
{
    public class PushNotifier
    {
        private readonly IRestClient _client;

        public PushNotifier(IRestClient client)
        {
            _client = client;
        }

        public void Send(NotificationPayload message, IQueryable<Device> devices)
        {
            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = devices.Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = devices.Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .ToList()
            };

            iosNotification.Send(_client);
            androidNotification.Send(_client);
        }
    }
}