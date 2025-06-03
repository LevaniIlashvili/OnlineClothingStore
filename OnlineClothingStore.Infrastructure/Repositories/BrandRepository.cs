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

        public async Task<Brand?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, Name
                FROM Brand
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Brand>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, Name
                FROM Brand";

            return await connection.QueryAsync<Brand>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT Id, Name
                           FROM Brand
                           WHERE Name = @Name";

            return await connection.QuerySingleOrDefaultAsync<Brand>(
                new CommandDefinition(sql, new { Name = name },cancellationToken: cancellationToken));
        }

        public async Task<Brand> AddAsync(Brand brand, CancellationToken cancellationToken = default
        )
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"INSERT INTO Brand (Name, CreatedAt, CreatedBy)
                           OUTPUT INSERTED.Id, INSERTED.Name
                           VALUES (@Name, @CreatedAt, @CreatedBy)";

            return await connection.QuerySingleAsync<Brand>(
                new CommandDefinition(sql, brand, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(Brand brand, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE Brand
                SET Name = @Name,
                    LastUpdatedAt = @LastUpdatedAt,
                    LastUpdatedBy = @LastUpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, brand, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(Brand brand, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "DELETE FROM Brand WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, brand, cancellationToken: cancellationToken));
        }
    }
}
