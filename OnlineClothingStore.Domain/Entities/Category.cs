namespace OnlineClothingStore.Domain.Entities
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public long? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
