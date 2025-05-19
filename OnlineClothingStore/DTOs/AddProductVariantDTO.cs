namespace OnlineClothingStore.DTOs
{
    public class AddProductVariantDTO
    {
        public int ProductId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Stock { get; set; }
    }
}
