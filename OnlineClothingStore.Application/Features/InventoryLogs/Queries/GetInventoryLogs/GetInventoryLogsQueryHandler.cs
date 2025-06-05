using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Common;

namespace OnlineClothingStore.Application.Features.InventoryLogs.Queries.GetInventoryLogs
{
    public class GetInventoryLogsQueryHandler : IRequestHandler<GetInventoryLogsQuery, List<InventoryLogDTO>>
    {
        private readonly IInventoryLogRepository _inventoryLogRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IMapper _mapper;

        public GetInventoryLogsQueryHandler(
            IInventoryLogRepository inventoryLogRepository,
            IProductVariantRepository productVariantRepository,
            IMapper mapper)
        {
            _inventoryLogRepository = inventoryLogRepository;
            _productVariantRepository = productVariantRepository;
            _mapper = mapper;
        }

        public async Task<List<InventoryLogDTO>> Handle(GetInventoryLogsQuery request, CancellationToken cancellationToken)
        {
            var inventoryLogs = (await _inventoryLogRepository.GetAllAsync(cancellationToken)).ToList();
            var inventoryLogDTOs = _mapper.Map<List<InventoryLogDTO>>(inventoryLogs);

            var variantIds = inventoryLogs.Select(log => log.ProductVariantId).Distinct();
            var productVariants = await _productVariantRepository.GetByIdsAsync(variantIds, cancellationToken);
            var productVariantDict = productVariants.ToDictionary(pv => pv.Id);

            for (int i = 0; i < inventoryLogs.Count; i++)
            {
                var log = inventoryLogs[i];

                if (productVariantDict.TryGetValue(log.ProductVariantId, out var variant))
                {
                    inventoryLogDTOs[i].ProductVariantSku = variant.Sku;
                }

                inventoryLogDTOs[i].ChangeType = ((InventoryLogChangeType)log.ChangeTypeId).ToString();
            }

            return inventoryLogDTOs;
        }
    }
}
