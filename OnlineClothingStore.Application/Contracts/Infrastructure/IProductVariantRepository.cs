using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface IProductVariantRepository
    {
        Task<ProductVariant?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductVariant> AddAsync(ProductVariant productVariant, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductVariant productVariant, CancellationToken cancellationToken = default);
        Task DeleteAsync(ProductVariant productVariant, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(long productId, CancellationToken cancellationToken = default);
        Task<ProductVariant?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    }
}
