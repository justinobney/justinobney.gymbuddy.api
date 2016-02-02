using System;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class DeleteUserCommand : IAsyncRequest<User>
    {
        public long Id { get; set; }
    }

    [DoNotValidate]
    public class DeleteUserCommandHandler : IAsyncRequestHandler<DeleteUserCommand, User>
    {
        private readonly UserRepository _userRepo;

        public DeleteUserCommandHandler(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> Handle(DeleteUserCommand message)
        {
            User user = await _userRepo.GetByIdAsync(message.Id);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            await _userRepo.DeleteAsync(user);
            return user;
        }
    }
}