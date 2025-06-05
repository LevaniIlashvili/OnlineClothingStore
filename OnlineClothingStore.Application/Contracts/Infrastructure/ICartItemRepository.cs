using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<CartItem>> GetByCartIdAsync(long cartId, CancellationToken cancellationToken = default);
        Task<CartItem> AddAsync(CartItem cartItem, CancellationToken cancellationToken = default);
        Task UpdateAsync(CartItem cartItem, CancellationToken cancellationToken = default);
        Task DeleteByCartIdAsync(long cartId, CancellationToken cancellationToken = default);
        Task DeleteAsync(CartItem cartItem, CancellationToken cancellationToken = default);
    }
}
