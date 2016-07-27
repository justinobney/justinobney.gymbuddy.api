using System;
using System.Collections.Generic;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Posts
{
    public class Post : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        
        public ICollection<PostContent> Contents { get; set; }
        public ICollection<PostKudos> Kudos { get; set; }
        public ICollection<PostComment> Comments { get; set; }


        public DateTime? Timestamp { get; set; }

        public User User { get; set; }
    }
}