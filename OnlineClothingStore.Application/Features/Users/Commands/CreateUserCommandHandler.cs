using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Users.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, long>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, ICartRepository cartRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<long> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingUser is not null)
            {
                throw new Exceptions.ConflictException("User with this email already exists");
            }

            var user = _mapper.Map<User>(request);
            user.PasswordHash = request.Password;
            user.RoleId = 1;
            user.CreatedAt = DateTime.UtcNow;

            var addedUser = await _userRepository.AddAsync(user, cancellationToken);

            if (addedUser is not null)
            {
                var cart = new Cart()
                {
                    UserId = addedUser.Id
                };
                cart.CreatedAt = DateTime.UtcNow;
                await _cartRepository.AddAsync(cart, cancellationToken);
            }

            return addedUser.Id;
        }
    }
}
