namespace OnlineClothingStore.Domain.Entities
{
    public class InventoryLogChangeType
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
    }
}
