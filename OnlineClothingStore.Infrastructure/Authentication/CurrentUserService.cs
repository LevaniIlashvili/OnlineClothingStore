using Microsoft.AspNetCore.Http;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using System.Security.Claims;

namespace OnlineClothingStore.Infrastructure.Authentication
{
    public class CurrentUserService : ICurrentUserService
    {
        public long UserId { get; }
        public string Email { get; }
        public string Role { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated is not true)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var id))
            {
                throw new UnauthorizedAccessException("Invalid or missing user ID.");
            }

            UserId = id;
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? throw new UnauthorizedAccessException("Email not found.");
            Role = user.FindFirst(ClaimTypes.Role)?.Value ?? throw new UnauthorizedAccessException("Role not found.");
        }
    }
}
