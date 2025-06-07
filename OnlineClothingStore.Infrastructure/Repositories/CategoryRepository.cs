using Dapper;
using OnlineClothingStore.Application;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CategoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Category?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT Id, Name, ParentCategoryId
                                 FROM Category
                                 WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Category>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT Id, Name, ParentCategoryId
                                 FROM Category";

            return await connection.QueryAsync<Category>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"Select Id, Name, ParentCategoryId
                           FROM Category
                           WHERE Name = @Name";

            return await connection.QuerySingleOrDefaultAsync<Category>(
                new CommandDefinition(sql, new { Name = name }, cancellationToken: cancellationToken));
        }

        public async Task<bool> HasChildrenAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT COUNT(*)
                           FROM Category
                           WHERE ParentCategoryId = @ParentCategoryId";
            var childrenCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { ParentCategoryId = id }, cancellationToken: cancellationToken));
            return childrenCount > 0;
        }

        public async Task<bool> HasProductsAsync(long categoryId, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT COUNT(*)
                           FROM Product
                           WHERE CategoryId = @CategoryId";
            var productCount = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { CategoryId = categoryId }, cancellationToken: cancellationToken));
            return productCount > 0;
        }

        public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO Category (Name, ParentCategoryId, CreatedAt, CreatedBy)
                OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.ParentCategoryId
                VALUES (@Name, @ParentCategoryId, @CreatedAt, @CreatedBy)";

            return await connection.QuerySingleAsync<Category>(
                new CommandDefinition(sql, category, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE Category
                SET Name = @Name,
                    ParentCategoryId = @ParentCategoryId,
                    LastUpdatedAt = @LastUpdatedAt,
                    LastUpdatedBy = @LastUpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, category, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "DELETE FROM Category WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, category, cancellationToken: cancellationToken));
        }
    }
}
