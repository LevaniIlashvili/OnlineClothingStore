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

        public async Task<Category?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT Id, Name, ParentCategoryId
                           FROM Category
                           WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<Category>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT Id, Name, ParentCategoryId
                           FROM Category";
            return await connection.QueryAsync<Category>(sql);
        }

        public async Task<Category> AddAsync(Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO Category (Name, ParentCategoryId, CreatedAt, CreatedBy)
                OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.ParentCategoryId
                VALUES (@Name, @ParentCategoryId, @CreatedAt, @CreatedBy)";
            return await connection.QuerySingleAsync<Category>(sql, category);
        }

        public async Task UpdateAsync(Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE Category
                SET Name = @Name,
                    ParentCategoryId = @ParentCategoryId,
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, category);
        }

        public async Task DeleteAsync(Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "DELETE FROM Category WHERE Id = @Id";
            await connection.ExecuteAsync(sql, category);
        }
    }
}
