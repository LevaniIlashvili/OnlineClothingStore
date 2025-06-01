using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Contracts.Infrastructure
{
    public interface IInventoryLogRepository
    {
        Task<InventoryLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryLog>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryLog>> GetByProductVariantIdAsync(long productVariantId, CancellationToken cancellationToken = default);
        Task<InventoryLog> AddAsync(InventoryLog inventoryLog, CancellationToken cancellationToken = default);
        Task UpdateAsync(InventoryLog inventoryLog, CancellationToken cancellationToken = default);
        Task DeleteAsync(InventoryLog inventoryLog, CancellationToken cancellationToken = default);
    }
}
