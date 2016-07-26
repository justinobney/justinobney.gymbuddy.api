using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Posts
{
    public class PostContent : IEntity
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public PostType Type { get; set; }
        public string Value { get; set; }
        public string MetaJson { get; set; }
    }
}