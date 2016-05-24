using System;
using System.Collections.ObjectModel;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using Newtonsoft.Json;

namespace justinobney.gymbuddy.api.Data.Posts
{
    public class Post : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        public string Title { get; set; }
        public PostType Type { get; set; }
        public string ContentJson { get; set; }
        public bool SkipPush { get; set; }
        public Collection<PostKudos> Kudos { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
    

    public class PostKudos : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PostId { get; set; }
    }
}