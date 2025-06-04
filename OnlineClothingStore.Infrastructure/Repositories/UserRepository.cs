using Dapper;
using OnlineClothingStore.Application;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, FirstName, LastName, Email, PasswordHash, PhoneNumber, RoleId
                FROM [User]
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<User>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, FirstName, LastName, Email, PasswordHash, PhoneNumber, RoleId
                FROM [User]";

            return await connection.QueryAsync<User>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT Id, FirstName, LastName, Email, PasswordHash, PhoneNumber, RoleId
                FROM [User]
                WHERE Email = @Email";

            return await connection.QuerySingleOrDefaultAsync<User>(
                new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken));
        }

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO [User] (
                    FirstName, LastName, Email, PasswordHash, PhoneNumber, RoleId,
                    CreatedAt, CreatedBy)
                OUTPUT INSERTED.Id, INSERTED.FirstName, INSERTED.LastName, INSERTED.Email,
                       INSERTED.PasswordHash, INSERTED.PhoneNumber, INSERTED.RoleId
                VALUES (
                    @FirstName, @LastName, @Email, @PasswordHash, @PhoneNumber, @RoleId,
                    @CreatedAt, @CreatedBy)";

            return await connection.QuerySingleAsync<User>(
                new CommandDefinition(sql, user, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                UPDATE [User]
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    PasswordHash = @PasswordHash,
                    PhoneNumber = @PhoneNumber,
                    RoleId = @RoleId,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, user, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"DELETE FROM [User] WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, new { user.Id }, cancellationToken: cancellationToken));
        }
    }
}
