using OnlineClothingStore.Domain.Common;

namespace OnlineClothingStore.Domain.Entities
{
    public class InventoryLog : AuditableEntity
    {
        public long Id { get; set; }
        public long ProductVariantId { get; set; }
        public long ChangeTypeId { get; set; }
        public int ChangeQuantity { get; set; }
        public int NewStockQuantity { get; set; }
        public string? Reason { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
