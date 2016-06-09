using System;
using System.Linq;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
{
    public class CreateAppointmentCommandMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<CreateAppointmentCommand, Appointment>()
                .ForMember(dest => dest.Exercises,
                    opts =>
                    {
                        opts.MapFrom(src => src.Exercises.Select(x => new AppointmentExercise {Name = x}));
                    });
            cfg.CreateMap<DateTime?, AppointmentTimeSlot>()
                .ForMember(dest => dest.Time, opts => opts.MapFrom(src => src));
        }
    }
}