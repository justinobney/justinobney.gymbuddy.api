using System;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Responses
{
    public class FriendshipListing
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }
        public string FriendshipKey { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime? FriendshipDate { get; set; }

        public string StatusText => Status.ToString();
        public string FriendName { get; set; }
        public string FriendProfilePictureUrl { get; set; }
        public bool Initiator { get; set; }
    }

    public class FriendshipListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<Friendship, FriendshipListing>()
                .ForMember(dest => dest.FriendName, opts => opts.MapFrom(src => src.Friend.Name))
                .ForMember(dest => dest.FriendProfilePictureUrl, opts => opts.MapFrom(src => src.Friend.ProfilePictureUrl));
        }
    }
}