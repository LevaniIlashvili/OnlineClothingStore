namespace OnlineClothingStore.Domain.Common
{
    public class AuditableEntity
    {
        public long? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
