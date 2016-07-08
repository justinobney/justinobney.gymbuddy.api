using AutoMapper;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Users.Dtos;
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
        private readonly IMapper _mapper;
        public UpdateNotificationSettingsCommandHandler(IMapper mapper, IDbSet<User> users)
        {
            _mapper = mapper;
            _users = users;
        }

        public User Handle(UpdateNotificationSettingsCommand message)
        {
            var user = _users.FirstOrDefault(x => x.Id == message.UserId);
            if (user != null)
            {
                user = _mapper.Map(message, user);

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

    public class UpdateNotificationSettingsCommandMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<NotificationSettingDto, UpdateNotificationSettingsCommand>();
            cfg.CreateMap<UpdateNotificationSettingsCommand, User>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}