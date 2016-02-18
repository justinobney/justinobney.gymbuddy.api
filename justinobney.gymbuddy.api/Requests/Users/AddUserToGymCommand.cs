using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class AddUserToGymCommand : IRequest<User>
    {
        public long UserId { get; set; }
        public long GymId { get; set; }
    }

    public class AddUserToGymCommandHandler : IRequestHandler<AddUserToGymCommand, User>
    {
        private readonly IDbSet<User> _users;
        private readonly IDbSet<Gym> _gyms;

        public AddUserToGymCommandHandler(IDbSet<User> users, IDbSet<Gym> gyms)
        {
            _users = users;
            _gyms = gyms;
        }

        public User Handle(AddUserToGymCommand message)
        {
            var user = _users.Find(message.UserId);
            var gym = _gyms.Find(message.GymId);

            // todo: move to validator
            ValidateEntities(user, gym);

            user.Gyms.Add(gym);

            return user;
        }
        

        private void ValidateEntities(User user, Gym gym)
        {
            if (user == null)
            {
                throw new ValidationException("Bad Data", new List<ValidationFailure>
                {
                    new ValidationFailure("deviceId", "User does not exist")
                });
            }
            if (gym == null)
            {
                throw new ValidationException("bad Data", new List<ValidationFailure>
                {
                    new ValidationFailure("gymId", "Gym does not exist")
                });
            }
            if (user.Gyms.Any(g => g.Id == gym.Id))
            {
                throw new ValidationException("Duplicate Entry", new List<ValidationFailure>
                {
                    new ValidationFailure("", "User already belongs to gym")
                });
            }
        }
    }

    public class AddUserToGymCommandValidator : AbstractValidator<AddUserToGymCommand>
    {
        public AddUserToGymCommandValidator()
        {
            RuleFor(x => x.GymId).GreaterThan(0);
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}