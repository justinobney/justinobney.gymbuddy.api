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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            command.UserId = CurrentUser.Id;
            var appointment = await _mediator.SendAsync(command);
            return CreatedAtRoute("DefaultApi", new { id = appointment.Id }, appointment);
        }
    }
}