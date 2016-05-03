using System;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Notifications;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
{
    public class CreateAppointmentNotifier : IPostRequestHandler<CreateAppointmentCommand, Appointment>
    {
        private readonly IDbSet<User> _users;
        private readonly IPushNotifier _pushNotifier;

        public CreateAppointmentNotifier(
            IDbSet<User> users,
            IPushNotifier pushNotifier
            )
        {
            _users = users;
            _pushNotifier = pushNotifier;
        }

        public void Notify(CreateAppointmentCommand request, Appointment response)
        {
            var notifyUsers = _users
                .Include(x => x.Devices)
                .Include(x => x.Gyms)
                .Where(x => x.Gyms.Any(y => y.Id == request.GymId))
                .Where(x => x.Id != request.UserId)
                .ToList();

            var additionalData = new AdditionalData { Type = NofiticationTypes.CreateAppointment };
            var message = new NotificationPayload(additionalData)
            {
                Title = "New Appointment Available",
                Alert = $"{response.User.Name} wants to work: {request.Title}"
            };

            _pushNotifier.Send(message, notifyUsers.SelectMany(x => x.Devices));
        }
        
    }
}