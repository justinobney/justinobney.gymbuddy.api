using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Gyms
{
    public class GetGymsCommand : IRequest<ICollection<GymListing>>
    {
        public long UserId { get; set; }
    }

    [DoNotValidate]
    [DoNotCommit]
    public class GetGymsCommandHandler : IRequestHandler<GetGymsCommand, ICollection<GymListing>>
    {
        private readonly IDbSet<Gym> _gyms;
        private readonly IDbSet<User> _users;

        public GetGymsCommandHandler(IDbSet<User> users, IDbSet<Gym> gyms)
        {
            _users = users;
            _gyms = gyms;
        }

        public ICollection<GymListing> Handle(GetGymsCommand message)
        {
            var user = _users.FirstOrDefault(x => x.Id == message.UserId);
            var userGymIds = user.Gyms.Select(x => x.Id).ToArray();

            return _gyms
                .ProjectTo<GymListing>(MappingConfig.Config, new { userGymIds })
                .ToList();
        }
    }
}