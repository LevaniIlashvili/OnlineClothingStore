using MediatR;

namespace OnlineClothingStore.Application.Features.Users.Commands
{
    public class CreateUserCommand : IRequest<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
