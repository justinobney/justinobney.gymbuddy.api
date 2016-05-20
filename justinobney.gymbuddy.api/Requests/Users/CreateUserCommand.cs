using System;
using System.Data.Entity;
using AutoMapper;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class CreateUserCommand : IRequest<User>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public FitnessLevel FitnessLevel { get; set; }
        public FitnessLevel FilterFitnessLevel { get; set; }
        public Gender Gender { get; set; }
        public Gender FilterGender { get; set; }
        public string DeviceId { get; set; }
        public string Platform { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IMapper _mapper;
        private readonly IDbSet<User> _users;

        public CreateUserCommandHandler(IMapper mapper, IDbSet<User> users)
        {
            _mapper = mapper;
            _users = users;
        }

        public User Handle(CreateUserCommand message)
        {
            var user = _mapper.Map<User>(message);
            user.CreatedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;

            user.Devices.Add(new Device
            {
                DeviceId = message.DeviceId,
                Platform = message.Platform,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            });

            _users.Add(user);

            return user;
        }
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.DeviceId).NotEmpty().WithMessage("Device Id is required");
            RuleFor(x => x.FitnessLevel).Must(value => value > 0).WithMessage("Fitness Level is required");
            RuleFor(x => x.Gender).Must(value => value > 0).WithMessage("Gender is required");
            RuleFor(x => x.FilterFitnessLevel).Must(value => value > 0).WithMessage("Min. Fitness Level is required");
            RuleFor(x => x.FilterGender).Must(value => value > 0).WithMessage("Gender Filter is required");
        }
    }

    public class CreateUserCommandMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<CreateUserCommand, User>();
        }
    }
}