using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using MediatR;
using RestSharp;

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

    public class CreateAppointmentPushNotifier : IPostRequestHandler<CreateAppointmentCommand, Appointment>
    {
        private readonly IDbSet<User> _users;
        private readonly IRestClient _client;

        public CreateAppointmentPushNotifier(IDbSet<User> users , IRestClient client)
        {
            _users = users;
            _client = client;
        }

        public void Notify(CreateAppointmentCommand request, Appointment response)
        {
            var notifyUsers = _users
                .Include(x => x.Devices)
                .Include(x => x.Gyms)
                .Where(x => x.Gyms.Any(y => y.Id == request.GymId))
                .Where(x => x.Id != request.UserId);

            var message = new NotificationPayload<object>(null)
            {
                Alert = $"{response.User.Name} wants to work: {request.Description}",
                Title = "New Appointment Available"
            };

            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = notifyUsers.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Tokens = notifyUsers.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            if (iosNotification.Tokens.Any())
            {
                //todo: log response
                var iosPushResponse = iosNotification.Send(_client);
                Debug.WriteLine(iosPushResponse.Content);
            }

            if (androidNotification.Tokens.Any())
            {
                //todo: log response
                var androidPushResponse = androidNotification.Send(_client);
                Debug.WriteLine(androidPushResponse.Content);
            }
        }
        
    }
}