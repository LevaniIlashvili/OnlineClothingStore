using Dapper;
using OnlineClothingStore.Application;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                 SELECT Id, Name, Description, Price, SkuPrefix,
                        CategoryId, BrandId
                 FROM Product
                 WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Product>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                SELECT Id, Name, Description, Price, SkuPrefix,
                       CategoryId, BrandId
                FROM Product
                WHERE Id IN @Ids";

            return await connection.QueryAsync<Product>(
                new CommandDefinition(sql, new { Ids = ids }, cancellationToken: cancellationToken));
        }


        public async Task<(IEnumerable<Product>, int)> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string sortBy = "CreatedAt",
            string sortDirection = "ASC",
            CancellationToken cancellationToken = default)
        {
            var validSortColumns = new HashSet<string> { "CreatedAt", "Name", "Price", };
            if (!validSortColumns.Contains(sortBy))
                sortBy = "CreatedAt";

            sortDirection = sortDirection.ToUpper() == "DESC" ? "DESC" : "ASC";

            using var connection = _connectionFactory.CreateConnection();

            string countSql = "SELECT COUNT(*) FROM Product";

            string dataSql = $@"
                 SELECT Id, Name, Description, Price, SkuPrefix,
                        CategoryId, BrandId 
                 FROM Product
                 ORDER BY {sortBy} {sortDirection}
                 OFFSET @Offset ROWS
                 FETCH NEXT @PageSize ROWS ONLY";


            var parameters = new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };

            var count = await connection.ExecuteScalarAsync<int>(countSql);

            var products = await connection.QueryAsync<Product>(
                new CommandDefinition(dataSql,parameters, cancellationToken: cancellationToken));

            return (products, count);
        }

        public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                 SELECT Id, Name, Description, Price, SkuPrefix,
                        CategoryId, BrandId
                 FROM Product
                 WHERE Name = @Name";

            return await connection.QuerySingleOrDefaultAsync<Product>(
                new CommandDefinition(sql, new { Name = name }, cancellationToken: cancellationToken));
        }

        public async Task<Product?> GetBySkuPrefixAsync(string skuPrefix, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                 SELECT Id, Name, SkuPrefix, Description, Price,
                        CategoryId, BrandId
                 FROM Product
                 WHERE SkuPrefix = @SkuPrefix";

            return await connection.QuerySingleOrDefaultAsync<Product>(
                new CommandDefinition(sql, new { SkuPrefix = skuPrefix }, cancellationToken: cancellationToken));
        }

        public async Task<Product?> GetByVariantIdAsync(long variantId, CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                 SELECT Id, Name, SkuPrefix, Description, Price,
                        CategoryId, BrandId
                 FROM Product
                 WHERE Id = (SELECT ProductId
                       FROM ProductVariant
                       WHERE Id = @VariantId)";

            return await connection.QuerySingleOrDefaultAsync<Product>(
                new CommandDefinition(sql, new { VariantId = variantId }, cancellationToken: cancellationToken));
        }

        public async Task<bool> ProductExistsAsync(string name, string skuPrefix, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                 SELECT 1
                 FROM Product
                 WHERE Name = @Name OR SkuPrefix = @SkuPrefix";

            var result = await connection.QuerySingleOrDefaultAsync<int?>(
                new CommandDefinition(sql, new { Name = name, SkuPrefix = skuPrefix }, cancellationToken: cancellationToken));

            return result.HasValue;
        }

        public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO Product (Name, Description, Price, SkuPrefix, 
                                     CategoryId, BrandId, 
                                     CreatedAt, CreatedBy)
                OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.Description, 
                       INSERTED.Price, INSERTED.SkuPrefix,
                       INSERTED.CategoryId, INSERTED.BrandId
                VALUES (@Name, @Description, @Price, @SkuPrefix, 
                        @CategoryId, @BrandId, 
                        @CreatedAt, @CreatedBy)";

            return await connection.QuerySingleAsync<Product>(
                new CommandDefinition(sql, product, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE Product
                SET Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    SkuPrefix = @SkuPrefix,
                    CategoryId = @CategoryId,
                    BrandId = @BrandId,
                    LastUpdatedAt = @LastUpdatedAt,
                    LastUpdatedBy = @LastUpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, product, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "DELETE FROM Product WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, product, cancellationToken: cancellationToken));
        }
    }
}
