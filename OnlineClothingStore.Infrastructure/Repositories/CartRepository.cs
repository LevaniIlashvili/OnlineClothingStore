using Dapper;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CartRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Cart?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, UserId
                FROM Cart
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Cart>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<Cart?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT Id, UserId FROM Cart WHERE UserId = @UserId";

            return await connection.QuerySingleOrDefaultAsync<Cart>(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<Cart>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT Id, UserId FROM Cart";

            return await connection.QueryAsync<Cart>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<Cart> AddAsync(Cart cart, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO Cart (UserId, CreatedAt, CreatedBy)
                OUTPUT INSERTED.Id, INSERTED.UserId
                VALUES (@UserId, @CreatedAt, @CreatedBy)";

            return await connection.QuerySingleAsync<Cart>(
                new CommandDefinition(sql, cart, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                UPDATE Cart
                SET UserId = @UserId,
                    LastUpdatedAt = @LastUpdatedAt,
                    LastUpdatedBy = @LastUpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, cart, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(Cart cart, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Cart WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, new { cart.Id }, cancellationToken: cancellationToken));
        }
    }
}
