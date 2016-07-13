using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Responses
{
    public class GymListing
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string Zipcode { get; set; }
        public int MemberCount { get; set; }

        public bool HasUserJoinedGym { get; set; }
    }

    public class GymListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            long[] userGymIds = new List<long>().ToArray();

            cfg.CreateMap<Gym, GymListing>()
                .ForMember(dest => dest.MemberCount, opts => opts.MapFrom(src=>src.Members.Count)) // TODO: Filter by my search criteria
                .ForMember(dest => dest.HasUserJoinedGym, opts => opts.MapFrom(src => userGymIds.Contains(src.Id)));
        }
    }
}