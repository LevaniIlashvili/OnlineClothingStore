using MediatR;

namespace OnlineClothingStore.Application.Features.Users.Queries
{
    public class LoginUserQuery : IRequest<string>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
