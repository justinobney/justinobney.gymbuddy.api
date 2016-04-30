using System;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Notifications
{
    public class Notification : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        public string Message { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string JsonPayload { get; set; }
        public string NotificationKey { get; set; }

        public bool Seen { get; set; }
        public bool SkipPush { get; set; }


        public DateTime? CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}