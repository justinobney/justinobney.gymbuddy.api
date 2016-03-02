using System;
using System.Collections.Generic;
using System.Data.Entity;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
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
}