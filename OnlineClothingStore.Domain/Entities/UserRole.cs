namespace OnlineClothingStore.Domain.Entities
{
    public class UserRole
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
    }

}
