using System;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Responses
{
    public class AppointmentGuestListing
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTimeSlotId { get; set; }
        public string UserName { get; set; }
        public string UserProfilePictureUrl { get; set; }
        public string Title { get; set; }
        public DateTime? Time { get; set; }

        public string Status { get; set; }
    }

    public class AppointmentGuestListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<AppointmentGuest, AppointmentGuestListing>()
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserProfilePictureUrl, opts => opts.MapFrom(src => src.User.ProfilePictureUrl))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Appointment.Title))
                .ForMember(dest => dest.Time, opts => opts.MapFrom(src => src.TimeSlot.Time));
        }
    }
}