using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
    }
}
