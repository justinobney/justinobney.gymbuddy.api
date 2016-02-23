using System.Collections.Generic;

namespace justinobney.gymbuddy.api.Requests.External
{

    public interface IIonicNotification
    {
    }

    public class IonicPushNotification
    {

        public IonicPushNotification(IIonicNotification notification)
        {
            Notification = notification;
        }

        public List<string> Tokens { get; set; }
        public bool Production { get; set; }
        public IIonicNotification Notification { get; set; }
    }
    
    public class NotificationPayload<T> : IIonicNotification
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
    }
}