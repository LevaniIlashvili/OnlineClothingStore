using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Users.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, long>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            ICartRepository cartRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper,
            ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<long> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateUserCommand for Email: {Email}", request.Email);

            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingUser is not null)
            {
                _logger.LogWarning("User creation failed. Email already exists: {Email}", request.Email);
                throw new Exceptions.ConflictException("User with this email already exists");
            }

            var user = _mapper.Map<User>(request);
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            user.RoleId = 2;
            user.CreatedAt = DateTime.UtcNow;

            var addedUser = await _userRepository.AddAsync(user, cancellationToken);

            if (addedUser is not null)
            {
                _logger.LogInformation("User created successfully with ID: {UserId} and Email: {Email}", addedUser.Id, request.Email);

                var cart = new Cart()
                {
                    UserId = addedUser.Id
                };
                cart.CreatedAt = DateTime.UtcNow;
                await _cartRepository.AddAsync(cart, cancellationToken);

                _logger.LogInformation("Cart created successfully for User ID: {UserId}", addedUser.Id);
            }

            return addedUser.Id;
        }
    }
}
