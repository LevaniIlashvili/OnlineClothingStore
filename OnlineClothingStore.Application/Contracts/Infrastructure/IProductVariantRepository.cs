using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface IProductVariantRepository
    {
        Task<IEnumerable<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductVariant?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(long productId, CancellationToken cancellationToken = default);
        Task<ProductVariant?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<ProductVariant> AddAsync(ProductVariant variant, CancellationToken cancellationToken = default);
        Task DeleteAsync(ProductVariant variant, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductVariant variant, CancellationToken cancellationToken = default);
    }
}