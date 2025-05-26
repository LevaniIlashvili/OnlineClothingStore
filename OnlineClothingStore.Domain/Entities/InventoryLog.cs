namespace OnlineClothingStore.Domain.Entities
{
    public class InventoryLog
    {
        public long Id { get; set; }
        public long ProductVariantId { get; set; }
        public long ChangeTypeId { get; set; }
        public int ChangeQuantity { get; set; }
        public int NewStockQuantity { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;
        public InventoryLogChangeType ChangeType { get; set; } = null!;
    }
}
