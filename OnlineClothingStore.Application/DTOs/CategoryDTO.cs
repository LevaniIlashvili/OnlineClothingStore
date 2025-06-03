namespace OnlineClothingStore.Application.DTOs
{
    public class CategoryDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public long? ParentCategoryId { get; set; }
    }
}
