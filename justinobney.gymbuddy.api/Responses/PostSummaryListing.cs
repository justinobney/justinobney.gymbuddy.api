using System;
using System.Linq;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Responses
{
    public class PostSummaryListing
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public PostContent TextContent { get; set; }
        public PostContent ImageContent { get; set; }

        public int KudosCount { get; set; }
        public int CommentCount { get; set; }
        public PostComment LastComment { get; set; }

        public DateTime? Timestamp { get; set; }

    }

    public class PostSummaryListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<Post, PostSummaryListing>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.TextContent, opts => opts.MapFrom(src => src.Contents.FirstOrDefault(x=>x.Type == PostType.Text)))
                .ForMember(dest => dest.ImageContent, opts => opts.MapFrom(src => src.Contents.FirstOrDefault(x=>x.Type == PostType.Image)))
                .ForMember(dest => dest.KudosCount, opts => opts.MapFrom(src => src.Kudos.Count))
                .ForMember(dest => dest.CommentCount, opts => opts.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.LastComment, opts => opts.MapFrom(src => src.Comments.LastOrDefault()));
        }
    }
}