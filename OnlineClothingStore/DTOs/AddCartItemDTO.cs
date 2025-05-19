namespace OnlineClothingStore.DTOs
{
    public class AddCartItemDTO
    {
        public int ProductId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
    }
}
