using MediatR;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Common;

namespace OnlineClothingStore.Application.Features.InventoryLogs.Commands
{
    public class CreateInventoryLogCommand : IRequest<InventoryLogDTO>
    {
        public long ProductVariantId { get; set; }
        public InventoryLogChangeType ChangeType { get; set; }
        public int ChangeQuantity { get; set; }
        public string? Reason { get; set; }
    }
}
