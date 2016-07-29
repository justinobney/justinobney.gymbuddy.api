using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class UpdateDeviceCommand : IRequest
    {
        public string DeviceId { get; set; }
        public string ClientVersion { get; set; }
        public string PushToken { get; set; }
        public string Platform { get; set; }
        public string FacebookUserId { get; set; }
    }

    [DoNotValidate]
    public class UpdateDeviceCommandHandler : RequestHandler<UpdateDeviceCommand>
    {
        private readonly IDbSet<Device> _devices;
        private readonly IDbSet<User> _users;

        public UpdateDeviceCommandHandler(IDbSet<Device> devices, IDbSet<User> users)
        {
            _devices = devices;
            _users = users;
        }

        protected override void HandleCore(UpdateDeviceCommand message)
        {
            var updateDateTime = DateTime.Now;
            var device = _devices.FirstOrDefault(x => x.DeviceId == message.DeviceId);
            if (device != null)
            {
                device.ModifiedAt = updateDateTime;
                device.PushToken = message.PushToken;
                device.Platform = message.Platform;

                if (!string.IsNullOrWhiteSpace(message.ClientVersion) && (device.ClientVersion != message.ClientVersion))
                    device.ClientVersion = message.ClientVersion;

                if (!string.IsNullOrWhiteSpace(message.FacebookUserId))
                {
                    var userChanged = false;
                    var user = _users.FirstOrDefault(x => x.Id == device.UserId);
                    if (user != null)
                    {
                        if (user.FacebookUserId != message.FacebookUserId)
                        {
                            user.FacebookUserId = message.FacebookUserId;
                            userChanged = true;
                        }

                        if (user.ProfilePictureUrl == null)
                        {
                            user.ProfilePictureUrl = $"https://graph.facebook.com/{message.FacebookUserId}/picture?type=large";
                            userChanged = true;
                        }

                        if (userChanged)
                            user.ModifiedAt = updateDateTime;
                    }
                }
            }
        }
    }
}