using Dapper;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductVariantRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ProductVariant?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, ProductId, Size, Color, Sku, StockQuantity, ImageUrl
                FROM ProductVariant
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<ProductVariant>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<ProductVariant>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                SELECT Id, ProductId, Size, Color, Sku, StockQuantity, ImageUrl
                FROM ProductVariant
                WHERE Id IN @Ids";

            return await connection.QueryAsync<ProductVariant>(
                new CommandDefinition(sql, new { Ids = ids }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, ProductId, Size, Color, Sku, StockQuantity, ImageUrl
                FROM ProductVariant";

            return await connection.QueryAsync<ProductVariant>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(long productId, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, ProductId, Size, Color, Sku, StockQuantity, ImageUrl
                FROM ProductVariant
                WHERE ProductId = @ProductId";

            return await connection.QueryAsync<ProductVariant>(
                new CommandDefinition(sql, new { ProductId = productId }, cancellationToken: cancellationToken));
        }

        public async Task<ProductVariant?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, ProductId, Size, Color, Sku, StockQuantity, ImageUrl
                FROM ProductVariant
                WHERE Sku = @Sku";

            return await connection.QuerySingleOrDefaultAsync<ProductVariant>(
                new CommandDefinition(sql, new { Sku = sku }, cancellationToken: cancellationToken));
        }

        public async Task<ProductVariant> AddAsync(ProductVariant variant, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO ProductVariant (
                    ProductId, Size, Color, Sku, StockQuantity, ImageUrl,
                    CreatedAt, CreatedBy)
                OUTPUT INSERTED.Id, INSERTED.ProductId, INSERTED.Size, INSERTED.Color,
                       INSERTED.Sku, INSERTED.StockQuantity, INSERTED.ImageUrl
                VALUES (
                    @ProductId, @Size, @Color, @Sku, @StockQuantity, @ImageUrl,
                    @CreatedAt, @CreatedBy)";

            return await connection.QuerySingleAsync<ProductVariant>(
                new CommandDefinition(sql, variant, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(ProductVariant variant, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE ProductVariant
                SET ProductId = @ProductId,
                    Size = @Size,
                    Color = @Color,
                    Sku = @Sku,
                    StockQuantity = @StockQuantity,
                    ImageUrl = @ImageUrl,
                    LastUpdatedAt = @LastUpdatedAt,
                    LastUpdatedBy = @LastUpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, variant, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(ProductVariant variant, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "DELETE FROM ProductVariant WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, variant, cancellationToken: cancellationToken));
        }
    }
}
