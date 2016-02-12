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
        public IHttpActionResult GetUsers()
        {
            var request = new GetAllByPredicateQuery<Appointment>
            {
                Predicate = appt =>
                    appt.UserId == CurrentUser.Id
                    && appt.TimeSlots.Any(ts => ts.Time > DateTime.UtcNow)
            };

            var appointment = _mediator.Send(request)
                .ProjectTo<AppointmentListing>(MappingConfig.Config)
                .ToList();

            return Ok(appointment);
        }

        // GET: api/Appointments/5
        [ResponseType(typeof(AppointmentListing))]
        public IHttpActionResult GetUser(int id)
        {
            var request = new GetAllByPredicateQuery<Appointment>
            {
                Predicate = u => u.Id == id
            };

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

            var appointment = await _mediator.SendAsync(command);
            return CreatedAtRoute("DefaultApi", new { id = appointment.Id }, appointment);
        }
    }
}