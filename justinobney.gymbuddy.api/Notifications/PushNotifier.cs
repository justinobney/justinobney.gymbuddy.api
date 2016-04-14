using System.Linq;
using justinobney.gymbuddy.api.Data.Devices;
using RestSharp;
using Serilog;

namespace justinobney.gymbuddy.api.Notifications
{
    public class PushNotifier
    {
        private readonly IRestClient _client;
        private readonly ILogger _logger;

        public PushNotifier(IRestClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
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

            var iosResp = iosNotification.Send(_client);
            var androidResp = androidNotification.Send(_client);

            _logger.Information($"iOS Notification: {iosResp.Content} ::: Tokens: {string.Join(", ", iosNotification.Tokens.ToArray())}");
            _logger.Information($"Android Notification: {androidResp.Content} ::: Tokens: {string.Join(", ", androidNotification.Tokens.ToArray())}");
        }
    }
}