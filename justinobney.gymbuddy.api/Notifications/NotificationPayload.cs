using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Notifications
{
    public class NotificationPayload<T> : INotificationPayload
    {
        public NotificationPayload(T payload)
        {
            Android = new Android<T>
            {
                Payload = payload
            };

            Ios = new Ios<T>
            {
                Payload = payload
            };
        }
        public string Alert { get; set; }
        public string Title { get; set; }
        public Android<T> Android { get; set; }
        public Ios<T> Ios { get; set; }
    }

    public class Android<T>
    {
        public T Payload { get; set; }
    }

    public class Ios<T>
    {
        public T Payload { get; set; }
        public int? Badge { get; set; }
    }
}