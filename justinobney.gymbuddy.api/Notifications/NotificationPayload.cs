namespace justinobney.gymbuddy.api.Notifications
{
    public class NotificationPayload
    {
        public NotificationPayload(AdditionalData payload)
        {
            Android = new Android
            {
                Payload = payload
            };

            Ios = new Ios
            {
                Payload = payload
            };
        }
        public string Message { get; set; }
        public string Title { get; set; }
        public Android Android { get; set; }
        public Ios Ios { get; set; }
    }

    public class Android
    {
        public AdditionalData Payload { get; set; }
        public string Sound = "default";
    }
    
    public class Ios
    {
        public AdditionalData Payload { get; set; }
        public string Sound = "default";
        public int? Badge { get; set; } = 0;
    }

    public class AdditionalData
    {
        public string Type { get; set; }
        public long AppointmentId { get; set; }
    }
}