using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Requests.Appointments;
using justinobney.gymbuddy.api.Requests.Appointments.AddAppointmentGuest;
using justinobney.gymbuddy.api.Requests.Appointments.Comments;
using justinobney.gymbuddy.api.Requests.Appointments.Confirm;
using justinobney.gymbuddy.api.Requests.Appointments.Create;
using justinobney.gymbuddy.api.Requests.Appointments.Delete;
using justinobney.gymbuddy.api.Requests.Appointments.Edit;
using justinobney.gymbuddy.api.Requests.Appointments.RemoveAppointmentGuest;
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

        [HttpGet]
        [Route("api/Appointments/My-Schedule")]
        [ResponseType(typeof(IEnumerable<AppointmentListing>))]
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

        [Route("api/Appointments/{id}/change-times")]
        [ResponseType(typeof(AppointmentListing))]
        [HttpPost]
        public IHttpActionResult AppointmentChangeTimes(long id, AppointmentChangeTimesCommand command)
        {
            command.UserId = CurrentUser.Id;
            command.AppointmentId = id;
            var appointment = _mediator.Send(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return Ok(listing);
        }

        // DELETE: api/Appointments/5
        [ResponseType(typeof(AppointmentListing))]
        public IHttpActionResult DeleteAppointment(long id)
        {
            var request = new DeleteAppointmentCommand {Id = id};

            var appointment = _mediator.Send(request);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(listing);
        }

        [Route("api/Appointments/{id}/Add-Guest/{timeSlotId}")]
        [ResponseType(typeof(AppointmentListing))]
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

        [Route("api/Appointments/{id}/Remove-Guest/{guestAppointmentId}")]
        [ResponseType(typeof(AppointmentListing))]
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

        [Route("api/Appointments/{id}/Confirm")]
        [ResponseType(typeof(AppointmentListing))]
        public IHttpActionResult ConfirmAppointment(long id, ConfirmAppointmentCommand command)
        {
            command.AppointmentId = id;

            var appointment = _mediator.Send(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return Ok(listing);
        }

        [Route("api/Appointments/{id}/On-My-Way")]
        [ResponseType(typeof(AppointmentListing))]
        public IHttpActionResult OnMyWay(long id, AppointmentOnMyWayCommand command)
        {
            command.AppointmentId = id;
            command.UserId = CurrentUser.Id;

            var appointment = _mediator.Send(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return Ok(listing);
        }

        [Route("api/Appointments/{id}/Comment")]
        [ResponseType(typeof(AppointmentListing))]
        public IHttpActionResult Comment(long id, AppointmentAddCommentCommand command)
        {
            command.AppointmentId = id;
            command.UserId = CurrentUser.Id;

            var appointment = _mediator.Send(command);
            var listing = MappingConfig.Instance.Map<AppointmentListing>(appointment);
            return Ok(listing);
        }

        [HttpPut]
        [Route("api/Appointments/{appointmentId}/Confirm-Guest/{appointmentGuestId}")]
        [ResponseType(typeof(AppointmentGuest))]
        public IHttpActionResult ConfirmAppointmentGuest(long appointmentId, long appointmentGuestId)
        {
            var command = new ConfirmAppointmentGuestCommand
            {
              AppointmentId  = appointmentId,
              AppointmentGuestId = appointmentGuestId
            };

            var guest = _mediator.Send(command);
            return Ok(MappingConfig.Instance.Map<AppointmentGuestListing>(guest));
        }
    }
}