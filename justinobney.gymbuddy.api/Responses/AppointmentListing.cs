using System;
using System.Collections.Generic;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Responses
{
    public class AppointmentListing
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string UserName { get; set; }
        public long? GymId { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? ConfirmedTime { get; set; }

        public List<AppointmentGuestListing> GuestList { get; set; }
        public List<AppointmentTimeSlot> TimeSlots { get; set; }
    }

    public class AppointmentGuestListing
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTimeSlotId { get; set; }

        public string UserName { get; set; }
        public DateTime? Time { get; set; }

        public string Status { get; set; }
    }

    public class AppointmentListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<Appointment, AppointmentListing>()
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status.ToString()))
                .ForMember(dest=>dest.UserName, opts => opts.MapFrom(src=>src.User.Name));
        }
    }

    public class AppointmentGuestListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<AppointmentGuest, AppointmentGuestListing>()
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Time, opts => opts.MapFrom(src => src.TimeSlot.Time));
        }
    }
}