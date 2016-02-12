using System;
using System.Linq;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Data.Appointments;
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
        private readonly AppointmentRepository _apptRepo;

        public DeleteUserCommandHandler(UserRepository userRepo, AppointmentRepository apptRepo)
        {
            _userRepo = userRepo;
            _apptRepo = apptRepo;
        }

        public async Task<User> Handle(DeleteUserCommand message)
        {
            var appts = _apptRepo.GetAll().Where(appt => appt.UserId == message.Id).ToList();

            foreach (var appt in appts)
            {
                await _apptRepo.DeleteAsync(appt);
            }

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