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

        public async Task<ProductVariant?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Id, ProductId, Size, Color, Sku, StockQuantity, ImageUrl,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM ProductVariant
                WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<ProductVariant>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Id, ProductId, Size, Color, Sku, StockQuantity, ImageUrl,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM ProductVariant";
            return await connection.QueryAsync<ProductVariant>(sql);
        }

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(long productId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Id, ProductId, Size, Color, Sku, StockQuantity, ImageUrl,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM ProductVariant
                WHERE ProductId = @ProductId";
            return await connection.QueryAsync<ProductVariant>(sql, new { ProductId = productId });
        }

        public async Task<ProductVariant> AddAsync(ProductVariant variant)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO ProductVariant (
                    ProductId, Size, Color, Sku, StockQuantity, ImageUrl,
                    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                OUTPUT INSERTED.Id, INSERTED.ProductId, INSERTED.Size, INSERTED.Color,
                       INSERTED.Sku, INSERTED.StockQuantity, INSERTED.ImageUrl,
                       INSERTED.CreatedAt, INSERTED.CreatedBy, INSERTED.UpdatedAt, INSERTED.UpdatedBy
                VALUES (
                    @ProductId, @Size, @Color, @Sku, @StockQuantity, @ImageUrl,
                    @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";
            return await connection.QuerySingleAsync<ProductVariant>(sql, variant);
        }

        public async Task UpdateAsync(ProductVariant variant)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE ProductVariant
                SET ProductId = @ProductId,
                    Size = @Size,
                    Color = @Color,
                    Sku = @Sku,
                    StockQuantity = @StockQuantity,
                    ImageUrl = @ImageUrl,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, variant);
        }

        public async Task DeleteAsync(ProductVariant variant)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM ProductVariant WHERE Id = @Id";
            await connection.ExecuteAsync(sql, variant);
        }
    }
}
