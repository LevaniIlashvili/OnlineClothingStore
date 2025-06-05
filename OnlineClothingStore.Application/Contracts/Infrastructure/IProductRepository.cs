using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
        Task<(IEnumerable<Product> products, int count)> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string sortBy = "Name",
            string sortDirection = "ASC",
            CancellationToken cancellationToken = default);
        Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Product?> GetBySkuPrefixAsync(string skuPrefix, CancellationToken cancellationToken = default);
        Task<bool> ProductExistsAsync(string name, string skuPrefix, CancellationToken cancellationToken = default);
        Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
    }
}
