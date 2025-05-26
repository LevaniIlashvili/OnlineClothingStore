using OnlineClothingStore.Domain.Common;

namespace OnlineClothingStore.Domain.Entities
{
    public class UserRole : AuditableEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
    }

}
