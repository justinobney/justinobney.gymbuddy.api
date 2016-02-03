using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class GetUserQuery : IAsyncRequest<ProfileListing>
    {
        public string DeviceId { get; set; }
    }

    [Authorize]
    public class GetUserQueryHandler : IAsyncRequestHandler<GetUserQuery, ProfileListing>
    {
        private readonly UserRepository _userRepo;

        public GetUserQueryHandler(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public Task<ProfileListing> Handle(GetUserQuery message)
        {
            return _userRepo.Find(user =>
                user.Devices
                    .Any(device => device.DeviceId == message.DeviceId)
                )
                .ProjectTo<ProfileListing>(MappingConfig.Config)
                .FirstOrDefaultAsync();
        }
    }

    public class GetUserQueryAuthorizer : IAuthorizer<GetUserQuery>
    {
        public bool Authorize(GetUserQuery message)
        {
            return true;
        }
    }

    public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
    {
        public GetUserQueryValidator()
        {
            RuleFor(request => request.DeviceId).NotEmpty();
        }
    }
}