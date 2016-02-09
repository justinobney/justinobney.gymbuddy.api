using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class CreateAppointmentCommand : IAsyncRequest<Appointment>
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public long? GymId { get; set; }
        public string Location { get; set; }
        
        public List<DateTime?> TimeSlots { get; set; }
    }

    public class CreateAppointmentCommandHandler : IAsyncRequestHandler<CreateAppointmentCommand, Appointment>
    {
        private readonly IMapper _mapper;
        private readonly AppointmentRepository _userRepo;

        public CreateAppointmentCommandHandler(IMapper mapper, AppointmentRepository userRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
        }

        public async Task<Appointment> Handle(CreateAppointmentCommand message)
        {
            var appointment = _mapper.Map(message, new Appointment());
            appointment.Status = AppointmentStatus.AwaitingGuests;
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.ModifiedAt = DateTime.UtcNow;

            await _userRepo.InsertAsync(appointment);
            return appointment;
        }
    }

    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentCommandValidator()
        {

        }
    }

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