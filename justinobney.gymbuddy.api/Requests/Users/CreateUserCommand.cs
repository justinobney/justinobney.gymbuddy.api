using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class CreateUserCommand : IAsyncRequest<User>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public FitnessLevel FitnessLevel { get; set; }
        public FitnessLevel FilterFitnessLevel { get; set; }
        public Gender Gender { get; set; }
        public Gender FilterGender { get; set; }
        public string DeviceId { get; set; }
    }

    public class CreateUserCommandHandler : IAsyncRequestHandler<CreateUserCommand, User>
    {
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepo;

        public CreateUserCommandHandler(IMapper mapper, UserRepository userRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
        }

        public async Task<User> Handle(CreateUserCommand message)
        {
            var user = _mapper.Map<User>(message);
            user.CreatedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;

            user.Devices.Add(new Device
            {
                DeviceId = message.DeviceId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            });

            await _userRepo.InsertAsync(user);

            return user;
        }
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.DeviceId).NotEmpty();
        }
    }

}