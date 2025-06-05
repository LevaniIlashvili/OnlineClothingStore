using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Common;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.InventoryLogs.Commands
{
    public class CreateInventoryLogCommandHandler : IRequestHandler<CreateInventoryLogCommand, InventoryLogDTO>
    {
        private readonly IInventoryLogRepository _inventoryLogRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IMapper _mapper;

        public CreateInventoryLogCommandHandler(
            IInventoryLogRepository inventoryLogRepository, 
            IProductVariantRepository productVariantRepository,
            IMapper mapper)
        {
            _inventoryLogRepository = inventoryLogRepository;
            _productVariantRepository = productVariantRepository;
            _mapper = mapper;
        }

        public async Task<InventoryLogDTO> Handle(CreateInventoryLogCommand request, CancellationToken cancellationToken)
        {
            var productVariant = await _productVariantRepository.GetByIdAsync(request.ProductVariantId, cancellationToken);

            if (productVariant is null)
            {
                throw new Exceptions.NotFoundException("Product variant not found");
            }

            if (request.ChangeQuantity is 0)
            {
                throw new Exceptions.BadRequestException("Quantity change cannot be 0");
            }

            if (productVariant.StockQuantity + request.ChangeQuantity < 0)
            {
                throw new Exceptions.BadRequestException("New stock quantity cannot be less than 0");
            }

            productVariant.StockQuantity += request.ChangeQuantity;
            productVariant.LastUpdatedAt = DateTime.UtcNow;

            await _productVariantRepository.UpdateAsync(productVariant, cancellationToken);

            var log = new InventoryLog()
            {
                ProductVariantId = productVariant.Id,
                ChangeQuantity = request.ChangeQuantity,
                NewStockQuantity =  productVariant.StockQuantity,
                ChangeTypeId = (long)request.ChangeType,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow,
            };


            log = await _inventoryLogRepository.AddAsync(log, cancellationToken);

            var inventoryLogDTO =  _mapper.Map<InventoryLogDTO>(log);
            inventoryLogDTO.ProductVariantSku = productVariant.Sku;
            inventoryLogDTO.ChangeType = ((InventoryLogChangeType)log.ChangeTypeId).ToString();

            return inventoryLogDTO;
        }
    }
}
