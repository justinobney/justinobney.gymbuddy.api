using AutoMapper.Internal;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using justinobney.gymbuddy.api.Responses;
using MediatR;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
            var gyms = _gyms.ToList();
            var gymList = MappingConfig.Instance.Map<List<GymListing>>(gyms);
            var user = _users.FirstOrDefault(x => x.Id == message.UserId);
            if (user?.Gyms != null && user.Gyms.Any())
            {
                var userGymIds = user.Gyms.Select(x => x.Id).ToList();
                gymList.Where(x => userGymIds.Contains(x.Id)).Each(x => x.HasUserJoinedGym = true);
            }
            return gymList;
        }
    }
}