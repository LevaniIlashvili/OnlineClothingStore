using Dapper;
using OnlineClothingStore.Application;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BrandRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Brand?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Id, Name
                FROM Brand
                WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<Brand>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Id, Name
                FROM Brand";
            return await connection.QueryAsync<Brand>(sql);
        }

        public async Task<Brand> AddAsync(Brand brand)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Brand (Name, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.CreatedAt, INSERTED.CreatedBy, INSERTED.UpdatedAt, INSERTED.UpdatedBy
                VALUES (@Name, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";
            return await connection.QuerySingleAsync<Brand>(sql, brand);
        }

        public async Task UpdateAsync(Brand brand)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Brand
                SET Name = @Name,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, brand);
        }

        public async Task DeleteAsync(Brand brand)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM Brand WHERE Id = @Id";
            await connection.ExecuteAsync(sql, brand);
        }
    }
}
