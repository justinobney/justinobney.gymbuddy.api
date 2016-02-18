using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Requests.Appointments;
using justinobney.gymbuddy.api.Requests.Generic;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class AppointmentsController : AuthenticatedController
    {
        private readonly IMediator _mediator;

        public AppointmentsController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Appointments
        [ResponseType(typeof(IEnumerable<AppointmentListing>))]
        public IHttpActionResult GetAppointments()
        {
            var gymIds = CurrentUser.Gyms.Select(g => g.Id).ToList();
            var request = new GetAllByPredicateQuery<Appointment>
            {
                Predicate = appt =>
                    gymIds.Contains(appt.GymId.Value)
                    && appt.TimeSlots.Any(ts => ts.Time > DateTime.UtcNow)
            };

            var appointment = _mediator.Send(request)
                .ProjectTo<AppointmentListing>(MappingConfig.Config)
                .ToList();

            return Ok(appointment);
        }

        // GET: api/Appointments/5
        [ResponseType(typeof(AppointmentListing))]
        public IHttpActionResult GetAppointment(int id)
        {
            var request = new GetAllByPredicateQuery<Appointment>(u => u.Id == id);

            var appointment = _mediator.Send(request)
                .ProjectTo<AppointmentListing>(MappingConfig.Config)
                .FirstOrDefault();

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }

        [ResponseType(typeof(AppointmentListing))]
        public async Task<IHttpActionResult> PostAppointment(CreateAppointmentCommand command)
        {
            command.UserId = CurrentUser.Id;
            var appointment = await _mediator.SendAsync(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return CreatedAtRoute("DefaultApi", new { id = listing.Id }, listing);
        }

        [ResponseType(typeof(AppointmentListing))]
        [Route("api/Appointments/{id}/Add-Guest/{timeSlotId}")]
        public async Task<IHttpActionResult> AddAppointmentGuest(long id, long timeSlotId)
        {
            var command = new AddAppointmentGuestCommand
            {
                UserId = CurrentUser.Id,
                AppointmentId = id,
                AppointmentTimeSlotId = timeSlotId
            };

            var appointment = _mediator.Send(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return Ok(listing);
        }

        [ResponseType(typeof(AppointmentListing))]
        [Route("api/Appointments/{id}/Remove-Guest/{guestAppointmentId}")]
        public async Task<IHttpActionResult> RemoveAppointmentGuest(long guestAppointmentId)
        {
            var command = new RemoveAppointmentGuestCommand
            {
                GuestAppointmentId = guestAppointmentId
            };

            var appointment = await _mediator.SendAsync(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return Ok(listing);
        }
    }
}