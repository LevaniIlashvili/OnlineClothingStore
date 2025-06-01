using Dapper;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderItemRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<OrderItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, OrderId, ProductVariantId, Quantity, PriceAtPurchase
                FROM OrderItem
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<OrderItem>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, OrderId, ProductVariantId, Quantity, PriceAtPurchase,
                FROM OrderItem";

            return await connection.QueryAsync<OrderItem>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, OrderId, ProductVariantId, Quantity, PriceAtPurchase,
                FROM OrderItem
                WHERE OrderId = @OrderId";

            return await connection.QueryAsync<OrderItem>(
                new CommandDefinition(sql, new { OrderId = orderId }, cancellationToken: cancellationToken));
        }

        public async Task<OrderItem> AddAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO OrderItem 
                    (OrderId, ProductVariantId, Quantity, PriceAtPurchase,
                     CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                OUTPUT INSERTED.Id, INSERTED.OrderId, INSERTED.ProductVariantId, INSERTED.Quantity, INSERTED.PriceAtPurchase
                VALUES 
                    (@OrderId, @ProductVariantId, @Quantity, @PriceAtPurchase,
                     @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

            return await connection.QuerySingleAsync<OrderItem>(
                new CommandDefinition(sql, orderItem, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE OrderItem
                SET OrderId = @OrderId,
                    ProductVariantId = @ProductVariantId,
                    Quantity = @Quantity,
                    PriceAtPurchase = @PriceAtPurchase,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, orderItem, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"DELETE FROM OrderItem WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, new { orderItem.Id }, cancellationToken: cancellationToken));
        }
    }
}
