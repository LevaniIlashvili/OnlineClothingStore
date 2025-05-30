using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default);
        Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default);
        Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
        Task DeleteAsync(Payment payment, CancellationToken cancellationToken = default);
    }
}
