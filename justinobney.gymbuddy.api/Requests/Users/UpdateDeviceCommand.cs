using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class UpdateDeviceCommand : IRequest
    {
        public string DeviceId { get; set; }
        public string PushToken { get; set; }
        public string Platform { get; set; }
    }

    [DoNotValidate]
    public class UpdateDeviceCommandHandler : RequestHandler<UpdateDeviceCommand>
    {
        private readonly IDbSet<Device> _devices;

        public UpdateDeviceCommandHandler(IDbSet<Device> devices)
        {
            _devices = devices;
        }

        protected override void HandleCore(UpdateDeviceCommand message)
        {
            var device = _devices.First(x => x.DeviceId == message.DeviceId);
            device.PushToken = message.PushToken;
        }
    }
}