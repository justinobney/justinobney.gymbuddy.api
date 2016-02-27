using System;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
{
    public class CreateAppointmentCommandMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<CreateAppointmentCommand, Appointment>();
            cfg.CreateMap<DateTime?, AppointmentTimeSlot>()
                .ForMember(dest => dest.Time, opts => opts.MapFrom(src => src));
        }
    }
}