using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Posts
{
    public class PostKudos : IEntity
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long UserId { get; set; }
    }
}