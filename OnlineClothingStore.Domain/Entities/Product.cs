namespace OnlineClothingStore.Domain.Entities
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string SkuPrefix { get; set; } = null!;
        public long CategoryId { get; set; }
        public long BrandId { get; set; }

        public Category Category { get; set; } = null!;
        public Brand Brand { get; set; } = null!;
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
