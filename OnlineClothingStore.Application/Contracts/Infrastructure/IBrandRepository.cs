using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Brand>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> HasProductsAsync(long brandId, CancellationToken cancellationToken = default);
        Task<Brand> AddAsync(Brand brand, CancellationToken cancellationToken = default);
        Task UpdateAsync(Brand brand, CancellationToken cancellationToken = default);
        Task DeleteAsync(Brand brand, CancellationToken cancellationToken = default);
    }
}
