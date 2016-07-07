using AutoMapper;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Responses
{
    public class ProfileFriendshipListing : ProfileListing
    {
        public FriendshipStatus Status { get; set; }
        public string StatusText => Status.ToString();
    }

    public class ProfileFriendshipListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<User, ProfileFriendshipListing>()
                .ForMember(dest => dest.FitnessLevel, opts => opts.MapFrom(src => src.FitnessLevel.ToString()))
                .ForMember(dest => dest.Gender, opts => opts.MapFrom(src => src.Gender.ToString()));
        }
    }
}