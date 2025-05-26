namespace OnlineClothingStore.Domain.Entities
{
    public class OrderItem
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }

        public Order Order { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
