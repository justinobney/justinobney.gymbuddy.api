using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class CreateAppointmentCommand : IRequest<Appointment>
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public long? GymId { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<DateTime?> TimeSlots { get; set; } = new List<DateTime?>();
    }

    [Commit]
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Appointment>
    {
        private readonly IMapper _mapper;
        private readonly IDbSet<Appointment> _appointments;

        public CreateAppointmentCommandHandler(IMapper mapper, IDbSet<Appointment> appointments)
        {
            _mapper = mapper;
            _appointments = appointments;
        }

        public Appointment Handle(CreateAppointmentCommand message)
        {
            var appointment = _mapper.Map(message, new Appointment());
            appointment.Status = AppointmentStatus.AwaitingGuests;
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.ModifiedAt = DateTime.UtcNow;

            _appointments.Add(appointment);
            
            return appointment;
        }
    }

    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.TimeSlots).Must(list => list.Any())
                .WithMessage("At least one time slot is required");
            
            Custom(command =>
            {
                if (command.GymId.HasValue == false && string.IsNullOrEmpty(command.Location))
                {
                    return new ValidationFailure("Location", "Gym or Location is required");
                }
                return null;
            });
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