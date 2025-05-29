using Dapper;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, UserId, OrderStatusId, OrderDate, TotalAmount, ShippingAddress,
                       CreatedAt, UpdatedAt
                FROM [Order]
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Order>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, UserId, OrderStatusId, OrderDate, TotalAmount, ShippingAddress,
                       CreatedAt, UpdatedAt
                FROM [Order]
                WHERE UserId = @UserId";

            return await connection.QueryAsync<Order>(new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, UserId, OrderStatusId, OrderDate, TotalAmount, ShippingAddress,
                       CreatedAt, UpdatedAt
                FROM [Order]";

            return await connection.QueryAsync<Order>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO [Order] 
                    (UserId, OrderStatusId, OrderDate, TotalAmount, ShippingAddress,
                     CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                OUTPUT INSERTED.Id, INSERTED.UserId, INSERTED.OrderStatusId, INSERTED.OrderDate,
                       INSERTED.TotalAmount, INSERTED.ShippingAddress, INSERTED.CreatedAt,
                       INSERTED.UpdatedAt
                VALUES
                    (@UserId, @OrderStatusId, @OrderDate, @TotalAmount, @ShippingAddress,
                     @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

            return await connection.QuerySingleAsync<Order>(new CommandDefinition(sql, order, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE [Order]
                SET UserId = @UserId,
                    OrderStatusId = @OrderStatusId,
                    OrderDate = @OrderDate,
                    TotalAmount = @TotalAmount,
                    ShippingAddress = @ShippingAddress,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(new CommandDefinition(sql, order, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"DELETE FROM [Order] WHERE Id = @Id";

            await connection.ExecuteAsync(new CommandDefinition(sql, new { order.Id }, cancellationToken: cancellationToken));
        }
    }
}
