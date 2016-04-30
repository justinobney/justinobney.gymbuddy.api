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
        public string Alert { get; set; }
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
        public int? Badge { get; set; }
    }

    public class AdditionalData
    {
        public string Type { get; set; }
    }
}