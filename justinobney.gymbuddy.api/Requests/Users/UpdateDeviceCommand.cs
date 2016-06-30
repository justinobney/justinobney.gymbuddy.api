using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class UpdateDeviceCommand : IRequest
    {
        public string DeviceId { get; set; }
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
            var device = _devices.First(x => x.DeviceId == message.DeviceId);
            device.PushToken = message.PushToken;
            device.Platform = message.Platform;

            if (message.FacebookUserId != null)
            {
                var user = _users.First(x => x.Id == device.UserId);
                user.FacebookUserId = message.FacebookUserId;
            }
        }
    }
}