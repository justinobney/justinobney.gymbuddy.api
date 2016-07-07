using AutoMapper;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Responses
{
    public class FacebookFriendshipListing
    {
        public long UserId { get; set; }
        public string FacebookUserId { get; set; }

        public FacebookFriendshipStatus Status { get; set; }
        public string StatusText => Status.ToString();
    }

    public class ProfileFriendshipListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<User, FacebookFriendshipListing>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
        }
    }
}