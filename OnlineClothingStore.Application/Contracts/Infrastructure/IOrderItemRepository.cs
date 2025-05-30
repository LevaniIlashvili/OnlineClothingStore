using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface IOrderItemRepository
    {
        Task<OrderItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<OrderItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default);
        Task<OrderItem> AddAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
        Task UpdateAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
        Task DeleteAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    }
}
