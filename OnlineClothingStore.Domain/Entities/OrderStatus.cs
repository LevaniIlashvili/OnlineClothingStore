namespace OnlineClothingStore.Domain.Entities
{
    public class OrderStatus
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
