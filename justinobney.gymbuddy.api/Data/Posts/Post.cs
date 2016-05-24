using System;
using System.Collections.ObjectModel;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        public static Post Create<T>(PostType type, T content)
        {
            return new Post
            {
                Type = type,
                ContentJson = JsonConvert.SerializeObject(content, StaticConfig.JsonSerializerSettings)
            };
        }
    }
    

    public class PostKudos : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PostId { get; set; }
    }
}