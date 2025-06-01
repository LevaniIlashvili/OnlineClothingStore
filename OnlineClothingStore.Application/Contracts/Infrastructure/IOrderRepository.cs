using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
        Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
        Task DeleteAsync(Order order, CancellationToken cancellationToken = default);
    }
}
