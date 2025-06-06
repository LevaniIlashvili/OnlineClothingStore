namespace OnlineClothingStore.Application.Contracts.Infrastructure.Authentication
{
    public interface ICurrentUserService
    {
        long UserId { get; }
        string Email { get; }
        string Role { get; }
    }
}
