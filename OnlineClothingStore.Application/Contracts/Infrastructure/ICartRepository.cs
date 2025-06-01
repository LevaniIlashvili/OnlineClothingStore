using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface ICartRepository
    {
        Task<Cart?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Cart>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Cart?> GetByUserIdAsync(long id, CancellationToken cancellationToken = default);
        Task<Cart> AddAsync(Cart cart, CancellationToken cancellationToken = default);
        Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default);
        Task DeleteAsync(Cart cart, CancellationToken cancellationToken = default);
    }
}
