using Dapper;
using OnlineClothingStore.Application;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class ProductRepository :  IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Product?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                 SELECT Id, Name, Description, Price, SkuPrefix,
                        CategoryId, BrandId
                 FROM Product
                 WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                 SELECT Id, Name, Description, Price, SkuPrefix,
                        CategoryId, BrandId
                 FROM Product";
            return await connection.QueryAsync<Product>(sql);
        }

        public async Task<Product> AddAsync(Product product)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO Product (Name, Description, Price, SkuPrefix, 
                                     CategoryId, BrandId, 
                                     CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.Description, 
                       INSERTED.Price, INSERTED.SkuPrefix,
                       INSERTED.CategoryId, INSERTED.BrandId,
                       INSERTED.CreatedAt, INSERTED.CreatedBy, INSERTED.UpdatedAt, INSERTED.UpdatedBy
                VALUES (@Name, @Description, @Price, @SkuPrefix, 
                        @CategoryId, @BrandId, 
                        @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";
            return await connection.QuerySingleAsync<Product>(sql, product);
        }

        public async Task UpdateAsync(Product product)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Product
                SET Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    SkuPrefix = @SkuPrefix,
                    CategoryId = @CategoryId,
                    BrandId = @BrandId,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, product);
        }

        public async Task DeleteAsync(Product product)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM Product WHERE Id = @Id";
            await connection.ExecuteAsync(sql, product);
        }
    }
}
