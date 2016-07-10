using FluentValidation;
using justinobney.gymbuddy.api.Data.Users;
using MediatR;
using System;
using System.Data.Entity;
using System.Linq;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class UpdateNotificationSettingsCommand : IRequest<User>
    {
        public long UserId { get; set; }

        public bool NewGymWorkoutNotifications { get; set; }
        public bool NewSquadWorkoutNotifications { get; set; }
        public bool SilenceAllNotifications { get; set; }
    }

    public class UpdateNotificationSettingsCommandHandler : IRequestHandler<UpdateNotificationSettingsCommand, User>
    {
        private readonly IDbSet<User> _users;

        public UpdateNotificationSettingsCommandHandler(IDbSet<User> users)
        {
            _users = users;
        }

        public User Handle(UpdateNotificationSettingsCommand message)
        {
            var user = _users.FirstOrDefault(x => x.Id == message.UserId);
            if (user != null)
            {
                user.NewGymWorkoutNotifications = message.NewGymWorkoutNotifications;
                user.NewSquadWorkoutNotifications = message.NewSquadWorkoutNotifications;
                user.SilenceAllNotifications = message.SilenceAllNotifications;

                user.ModifiedAt = DateTime.UtcNow;
            }
            return user;
        }
    }

    public class UpdateNotificationSettingsCommandValidator : AbstractValidator<UpdateNotificationSettingsCommand>
    {
        public UpdateNotificationSettingsCommandValidator()
        {
            RuleFor(x => x.UserId).NotNull().NotEqual(0);
        }
    }
    
}