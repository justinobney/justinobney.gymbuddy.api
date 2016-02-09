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
        public long? GymId { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public DateTime? ConfirmedTime { get; set; }

        public List<ProfileListing> GuestList { get; set; }
        public List<AppointmentTimeSlot> TimeSlots { get; set; }
    }

    public class AppointmentListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<Appointment, AppointmentListing>()
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status.ToString()));
        }
    }
}