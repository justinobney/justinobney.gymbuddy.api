using System.Collections.Generic;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Responses
{
    public class ProfileListing
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FitnessLevel { get; set; }
        public string Gender { get; set; }
        public List<GymListing> Gyms { get; set; }
    }

    public class ProfileListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<User, ProfileListing>()
                .ForMember(dest => dest.FitnessLevel, opts => opts.MapFrom(src => src.FitnessLevel.ToString()))
                .ForMember(dest => dest.Gender, opts => opts.MapFrom(src => src.Gender.ToString()));
        }
    }
}