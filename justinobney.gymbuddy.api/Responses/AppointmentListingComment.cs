using System;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Responses
{
    public class AppointmentListingComment
    {
        public string Text { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class AppointmentListingCommentMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<AppointmentComment, AppointmentListingComment>()
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.User.Name));
        }
    }
}