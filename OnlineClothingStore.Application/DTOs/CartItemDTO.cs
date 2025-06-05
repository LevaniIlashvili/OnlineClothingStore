namespace OnlineClothingStore.Application.DTOs
{
    public class CartItemDTO
    {
        public long Id { get; set; }
        public long CartId { get; set; }
        public long ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
