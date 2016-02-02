using System.Threading.Tasks;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class GetUserQuery : IAsyncRequest<User>
    {
        public long Id { get; set; }
    }

    [Authorize]
    public class GetUserQueryHandler : IAsyncRequestHandler<GetUserQuery, User>
    {
        private readonly UserRepository _userRepo;

        public GetUserQueryHandler(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public Task<User> Handle(GetUserQuery message)
        {
            return _userRepo.GetByIdAsync(message.Id);
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
            RuleFor(request => request.Id).GreaterThan(0);
        }
    }
}