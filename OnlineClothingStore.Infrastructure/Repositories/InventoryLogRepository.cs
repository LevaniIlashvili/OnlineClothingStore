using Dapper;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class InventoryLogRepository : IInventoryLogRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public InventoryLogRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<InventoryLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, ProductVariantId, ChangeTypeId, ChangeQuantity, NewStockQuantity, Reason,
                       CreatedAt, CreatedBy
                FROM InventoryLog
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<InventoryLog>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<InventoryLog>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, ProductVariantId, ChangeTypeId, ChangeQuantity, NewStockQuantity, Reason,
                       CreatedAt, CreatedBy
                FROM InventoryLog";

            return await connection.QueryAsync<InventoryLog>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<InventoryLog>> GetByProductVariantIdAsync(long productVariantId, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, ProductVariantId, ChangeTypeId, ChangeQuantity, NewStockQuantity, Reason,
                       CreatedAt, CreatedBy
                FROM InventoryLog
                WHERE ProductVariantId = @ProductVariantId";

            return await connection.QueryAsync<InventoryLog>(
                new CommandDefinition(sql, new { ProductVariantId = productVariantId }, cancellationToken: cancellationToken));
        }

        public async Task<InventoryLog> AddAsync(InventoryLog inventoryLog, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO InventoryLog (
                    ProductVariantId, ChangeTypeId, ChangeQuantity, NewStockQuantity, Reason,
                    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                OUTPUT INSERTED.Id, INSERTED.ProductVariantId, INSERTED.ChangeTypeId, INSERTED.ChangeQuantity,
                       INSERTED.NewStockQuantity, INSERTED.Reason, INSERTED.CreatedAt, INSERTED.CreatedBy
                VALUES (
                    @ProductVariantId, @ChangeTypeId, @ChangeQuantity, @NewStockQuantity, @Reason,
                    @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

            return await connection.QuerySingleAsync<InventoryLog>(
                new CommandDefinition(sql, inventoryLog, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(InventoryLog inventoryLog, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE InventoryLog
                SET ProductVariantId = @ProductVariantId,
                    ChangeTypeId = @ChangeTypeId,
                    ChangeQuantity = @ChangeQuantity,
                    NewStockQuantity = @NewStockQuantity,
                    Reason = @Reason,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, inventoryLog, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(InventoryLog inventoryLog, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"DELETE FROM InventoryLog WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, new { inventoryLog.Id }, cancellationToken: cancellationToken));
        }
    }
}
