namespace OnlineClothingStore.Application.DTOs
{
    public class ProductVariantDTO
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Size { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
