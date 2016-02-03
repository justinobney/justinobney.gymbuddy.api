using System.Linq;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class GetUsersQuery : IRequest<IQueryable<ProfileListing>>
    {
    }

    [DoNotValidate]
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IQueryable<ProfileListing>>
    {
        private readonly UserRepository _userRepo;

        public GetUsersQueryHandler(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IQueryable<ProfileListing> Handle(GetUsersQuery message)
        {
            return _userRepo.GetAll()
                .ProjectTo<ProfileListing>(MappingConfig.Config);
        }
    }
}