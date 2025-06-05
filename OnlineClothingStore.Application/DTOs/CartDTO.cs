namespace OnlineClothingStore.Application.DTOs
{
    public class CartDTO
    {
        public long Id { get; set; }
        public ICollection<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
    }
}
