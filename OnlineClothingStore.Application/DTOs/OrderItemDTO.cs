namespace OnlineClothingStore.Application.DTOs
{
    public class OrderItemDTO
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}
