namespace OnlineClothingStore.Application.DTOs
{
    public class ProductDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string SkuPrefix { get; set; } = null!;
        public long CategoryId { get; set; }
        public long BrandId { get; set; }
    }
}
