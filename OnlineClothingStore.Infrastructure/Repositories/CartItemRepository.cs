using Dapper;
using OnlineClothingStore.Application;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CartItemRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<CartItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, CartId, ProductVariantId, Quantity
                FROM CartItem
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<CartItem>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<CartItem>> GetByCartIdAsync(long cartId, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, CartId, ProductVariantId, Quantity
                FROM CartItem
                WHERE CartId = @CartId";

            return await connection.QueryAsync<CartItem>(new CommandDefinition(sql, new { CartId = cartId }, cancellationToken: cancellationToken));
        }

        public async Task<CartItem> AddAsync(CartItem cartItem, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO CartItem (CartId, ProductVariantId, Quantity, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                OUTPUT INSERTED.Id, INSERTED.CartId, INSERTED.ProductVariantId, INSERTED.Quantity
                VALUES (@CartId, @ProductVariantId, @Quantity, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

            return await connection.QuerySingleAsync<CartItem>(new CommandDefinition(sql, cartItem, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(CartItem cartItem, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE CartItem
                SET ProductVariantId = @ProductVariantId,
                    Quantity = @Quantity,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(new CommandDefinition(sql, cartItem, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(CartItem cartItem, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "DELETE FROM CartItem WHERE Id = @Id";

            await connection.ExecuteAsync(new CommandDefinition(sql, new { cartItem.Id }, cancellationToken: cancellationToken));
        }
    }
}
