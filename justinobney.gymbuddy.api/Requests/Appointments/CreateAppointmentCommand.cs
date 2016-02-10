using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
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
        public string Title { get; set; }
        public string Description { get; set; }

        public List<DateTime?> TimeSlots { get; set; } = new List<DateTime?>();
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
        private readonly UserRepository _userRepo;

        public CreateAppointmentCommandValidator(UserRepository userRepo)
        {
            _userRepo = userRepo;
            

            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.TimeSlots).Must(list => list.Any())
                .WithMessage("At least one time slow is required");
            
            Custom(command =>
            {
                if (command.GymId.HasValue == false && string.IsNullOrEmpty(command.Location))
                {
                    return new ValidationFailure("Location", "Gym or Location is requires");
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