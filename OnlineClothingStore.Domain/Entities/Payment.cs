using OnlineClothingStore.Domain.Common;

namespace OnlineClothingStore.Domain.Entities
{
    public class Payment : AuditableEntity
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? TransactionId { get; set; }

        public Order Order { get; set; } = null!;
    }
}
