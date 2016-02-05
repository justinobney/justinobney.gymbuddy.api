using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class AddUserToGymCommand : IAsyncRequest<User>
    {
        public string DeviceId { get; set; }
        public long GymId { get; set; }
    }

    public class AddUserToGymCommandHandler : IAsyncRequestHandler<AddUserToGymCommand, User>
    {
        private readonly UserRepository _userRepo;
        private readonly GymRepository _gymRepo;

        public AddUserToGymCommandHandler(UserRepository userRepo, GymRepository gymRepo)
        {
            _userRepo = userRepo;
            _gymRepo = gymRepo;
        }

        async Task<User> IAsyncRequestHandler<AddUserToGymCommand, User>.Handle(AddUserToGymCommand message)
        {
            var user = _userRepo.GetAll()
                .Include(u => u.Gyms)
                .FirstOrDefault(u =>
                    u.Devices.Any(d => d.DeviceId == message.DeviceId));

            var gym = await _gymRepo.GetByIdAsync(message.GymId);

            EnsureEntitesExist(user, gym);

            user.Gyms.Add(gym);
            await _userRepo.UpdateAsync(user);

            return user;
        }

        private void EnsureEntitesExist(User user, Gym gym)
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
            RuleFor(x => x.DeviceId).NotEmpty();
        }
    }
}