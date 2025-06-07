using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Users.Queries
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<LoginUserQueryHandler> _logger;

        public LoginUserQueryHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<LoginUserQueryHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<string> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling LoginUserQuery for Email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("Login failed: User not found for Email: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var isValidPassword = _passwordHasher.VerifyPassword(user.PasswordHash, request.Password);

            if (isValidPassword is false)
            {
                _logger.LogWarning("Login failed: Invalid password for Email: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            _logger.LogInformation("Login successful for Email: {Email}", request.Email);

            return token;
        }
    }
}
