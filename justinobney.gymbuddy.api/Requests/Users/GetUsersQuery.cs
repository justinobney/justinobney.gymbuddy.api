using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class GetUsersQuery : IRequest<IQueryable<User>>
    {
    }

    [DoNotValidate]
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IQueryable<User>>
    {
        private readonly UserRepository _userRepo;

        public GetUsersQueryHandler(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IQueryable<User> Handle(GetUsersQuery message)
        {
            return _userRepo.GetAll().Include(u=>u.Devices);
        }
    }
}