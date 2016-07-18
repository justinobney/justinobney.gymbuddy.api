using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hangfire;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
{
    public class AppointmentReminder : IPostRequestHandler<CreateAppointmentCommand, Appointment>
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly IPushNotifier _pushNotifier;
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _guests;
        private readonly IDbSet<User> _users;

        public AppointmentReminder(
            IBackgroundJobClient jobClient,
            IPushNotifier pushNotifier,
            IDbSet<Appointment>  appointments,
            IDbSet<AppointmentGuest> guests,
            IDbSet<User>  users
            )
        {
            _jobClient = jobClient;
            _pushNotifier = pushNotifier;
            _appointments = appointments;
            _guests = guests;
            _users = users;
        }

        public void Notify(CreateAppointmentCommand request, Appointment response)
        {
            var timeslotTime = response.TimeSlots.First().Time.Value;

            // Do not schedule reminders for workouts created less
            // less than 2 hours in the future
            if (DateTime.UtcNow.AddHours(2) > timeslotTime)
            {
                return;
            }

            var reminderTime = timeslotTime.AddHours(-1).ToFileTimeUtc();

            _jobClient.Schedule(() => SendNotification(response.Id), DateTimeOffset.FromFileTime(reminderTime));
        }

        public void SendNotification(long appointmentId)
        {
            var appt = _appointments
                .Include(x=>x.User.Devices)
                .Include(x=>x.TimeSlots)
                .FirstOrDefault(x => x.Id == appointmentId);

            if (appt == null || appt.TimeSlots.First().Time < DateTime.UtcNow)
            {
                return;
            }

            var notifyUsers = _guests
                .Include(x => x.User.Devices)
                .Where(x => x.AppointmentId == appointmentId && x.Status == AppointmentGuestStatus.Confirmed)
                .Select(x=>x.User)
                .ToList()
                .Concat(new List<User> {appt.User});
            
            var additionalData = new AdditionalData
            {
                Type = NofiticationTypes.CreateAppointment,
                AppointmentId = appointmentId
            };
            var message = new NotificationPayload(additionalData)
            {
                Title = "Upcoming Workout",
                Message = "Upcoming Workout"
            };

            _pushNotifier.Send(message, notifyUsers.SelectMany(x => x.Devices));
        }
    }
}