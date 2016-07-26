using System;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Posts
{
    public class PostComments : IEntity
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long UserId { get; set; }
        public string Value { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}