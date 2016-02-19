using System.CodeDom;
using System.Data.Entity;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Gyms
{
    public class GymPeekUsersCommand : IRequest<IQueryable<User>>
    {
        public User User { get; set; }
        public long GymId { get; set; }
    }

    
    public class GymPeekUsersCommandHandler : IRequestHandler<GymPeekUsersCommand, IQueryable<User>>
    {
        private readonly IDbSet<User> _users;

        public GymPeekUsersCommandHandler(IDbSet<User> users)
        {
            _users = users;
        }

        public IQueryable<User> Handle(GymPeekUsersCommand message)
        {
            return _users.Where(UserPredicates.RestrictMember(message.User, message.GymId));
        }
    }

    public class GymPeekUsersCommandValidator : AbstractValidator<GymPeekUsersCommand>
    {
        public GymPeekUsersCommandValidator()
        {
            RuleFor(x => x.User).NotNull();

            RuleFor(x => x.GymId).GreaterThan(0);
        }
    }
}