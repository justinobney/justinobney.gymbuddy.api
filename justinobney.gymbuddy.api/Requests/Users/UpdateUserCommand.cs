using System;
using System.Threading.Tasks;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class UpdateUserCommand : IAsyncRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public FitnessLevel FitnessLevel { get; set; }
        public FitnessLevel FilterFitnessLevel { get; set; }
        public Gender Gender { get; set; }
        public Gender FilterGender { get; set; }
    }

    [DoNotValidate]
    public class UpdateUserCommandHandler : AsyncRequestHandler<UpdateUserCommand>
    {
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepo;

        public UpdateUserCommandHandler(IMapper mapper, UserRepository userRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
        }

        protected override async Task HandleCore(UpdateUserCommand message)
        {
            var user = await _userRepo.GetByIdAsync(message.Id);
            _mapper.Map(message, user);
            user.ModifiedAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);
        }
    }

    public class UpdateUserCommandMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<UpdateUserCommand, User>()
                .ForMember(dest => dest.CreatedAt, opts => opts.Ignore());
        }
    }
}