using System.Collections.Generic;
using System.Linq;
using Hangfire;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Interfaces;
using RestSharp;
using Serilog;

namespace justinobney.gymbuddy.api.Notifications
{
    public class PushNotifier : IPushNotifier
    {
        private readonly IRestClient _client;
        private readonly ILogger _logger;
        
        public PushNotifier(IRestClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public void Send(NotificationPayload message, IEnumerable<Device> devices)
        {
            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = devices.Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .Distinct()
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = devices.Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .Distinct()
                    .ToList()
            };

            BackgroundJob.Enqueue(() => ProcessNotification(iosNotification));
            BackgroundJob.Enqueue(() => ProcessNotification(androidNotification));
        }

        public void ProcessNotification(IonicPushNotification notification)
        {
            var resp = notification.Send(_client);
            _logger.Information($"Notification: {resp.Content} ::: Tokens: {string.Join(", ", notification.Tokens.ToArray())}");
        }
    }
}