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
        public string GymName { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string UserProfilePictureUrl { get; set; }
        public DateTime? ConfirmedTime { get; set; }

        public List<AppointmentGuestListing> GuestList { get; set; }
        public List<AppointmentTimeSlot> TimeSlots { get; set; }
        public List<AppointmentListingComment> Comments { get; set; }
        public List<AppointmentExercise> Exercises { get; set; }
    }

    public class AppointmentListingMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<Appointment, AppointmentListing>()
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserProfilePictureUrl, opts => opts.MapFrom(src => src.User.ProfilePictureUrl))
                .ForMember(dest => dest.GymName, opts => opts.MapFrom(src => src.Gym.Name));
        }
    }
}