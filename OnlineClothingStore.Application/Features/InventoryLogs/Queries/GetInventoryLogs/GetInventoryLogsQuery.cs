using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.InventoryLogs.Queries.GetInventoryLogs
{
    public class GetInventoryLogsQuery : IRequest<List<InventoryLogDTO>> { }
}
