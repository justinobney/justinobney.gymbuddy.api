using System.Collections.Generic;
using System.Linq;
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
            var request = new GetAvailableAppointmentsForUserQuery
            {
                UserId = CurrentUser.Id,
                GymIds = gymIds
            };

            var appointment = _mediator.Send(request)
                .ProjectTo<AppointmentListing>(MappingConfig.Config)
                .ToList();

            return Ok(appointment);
        }

        [ResponseType(typeof(IEnumerable<AppointmentListing>))]
        [Route("api/Appointments/My-Schedule")]
        [HttpGet]
        public IHttpActionResult GetMySchedule()
        {
            var request = new GetScheduledAppointmentsForUserQuery
            {
                UserId = CurrentUser.Id
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
        public IHttpActionResult PostAppointment(CreateAppointmentCommand command)
        {
            command.UserId = CurrentUser.Id;
            var appointment =  _mediator.Send(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return CreatedAtRoute("DefaultApi", new { id = listing.Id }, listing);
        }

        [ResponseType(typeof(AppointmentListing))]
        [Route("api/Appointments/{id}/Add-Guest/{timeSlotId}")]
        public IHttpActionResult AddAppointmentGuest(long id, long timeSlotId)
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
        public IHttpActionResult RemoveAppointmentGuest(long guestAppointmentId)
        {
            var command = new RemoveAppointmentGuestCommand
            {
                GuestAppointmentId = guestAppointmentId
            };

            var appointment = _mediator.Send(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return Ok(listing);
        }

        [ResponseType(typeof(AppointmentListing))]
        [Route("api/Appointments/{id}/Confirm")]
        public IHttpActionResult ConfirmAppointment(long id, ConfirmAppointmentCommand command)
        {
            command.AppointmentId = id;

            var appointment = _mediator.Send(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return Ok(listing);
        }
    }
}