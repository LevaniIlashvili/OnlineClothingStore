using Dapper;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PaymentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Payment?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, OrderId, PaymentDate, Amount, PaymentMethod, TransactionId, CreatedAt
                FROM Payment
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Payment>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, OrderId, PaymentDate, Amount, PaymentMethod, TransactionId, CreatedAt
                FROM Payment";

            return await connection.QueryAsync<Payment>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<Payment>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT Id, OrderId, PaymentDate, Amount, PaymentMethod, TransactionId, CreatedAt
                FROM Payment
                WHERE OrderId = @OrderId";

            return await connection.QueryAsync<Payment>(
                new CommandDefinition(sql, new { OrderId = orderId }, cancellationToken: cancellationToken));
        }

        public async Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO Payment (
                    OrderId, PaymentDate, Amount, PaymentMethod, TransactionId,
                    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                OUTPUT INSERTED.Id, INSERTED.OrderId, INSERTED.PaymentDate, INSERTED.Amount,
                       INSERTED.PaymentMethod, INSERTED.TransactionId, INSERTED.CreatedAt
                VALUES (
                    @OrderId, @PaymentDate, @Amount, @PaymentMethod, @TransactionId,
                    @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

            return await connection.QuerySingleAsync<Payment>(
                new CommandDefinition(sql, payment, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                UPDATE Payment
                SET OrderId = @OrderId,
                    PaymentDate = @PaymentDate,
                    Amount = @Amount,
                    PaymentMethod = @PaymentMethod,
                    TransactionId = @TransactionId,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, payment, cancellationToken: cancellationToken));
        }

        public async Task DeleteAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"DELETE FROM Payment WHERE Id = @Id";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, new { payment.Id }, cancellationToken: cancellationToken));
        }
    }
}
