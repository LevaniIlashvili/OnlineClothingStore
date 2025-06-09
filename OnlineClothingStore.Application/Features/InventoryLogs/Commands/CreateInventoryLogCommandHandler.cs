using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Common;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.InventoryLogs.Commands
{
    public class CreateInventoryLogCommandHandler : IRequestHandler<CreateInventoryLogCommand, InventoryLogDTO>
    {
        private readonly IInventoryLogRepository _inventoryLogRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateInventoryLogCommandHandler> _logger;

        public CreateInventoryLogCommandHandler(
            IInventoryLogRepository inventoryLogRepository,
            IProductVariantRepository productVariantRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<CreateInventoryLogCommandHandler> logger)
        {
            _inventoryLogRepository = inventoryLogRepository;
            _productVariantRepository = productVariantRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<InventoryLogDTO> Handle(CreateInventoryLogCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling CreateInventoryLogCommand for ProductVariant ID: {ProductVariantId} by User: {UserId}", request.ProductVariantId, userId);

            var productVariant = await _productVariantRepository.GetByIdAsync(request.ProductVariantId, cancellationToken);
            if (productVariant is null)
            {
                _logger.LogWarning("Product variant not found with ID: {ProductVariantId}", request.ProductVariantId);
                throw new Exceptions.NotFoundException("Product variant not found");
            }

            if (request.ChangeQuantity is 0)
            {
                _logger.LogWarning("Attempted to create inventory log with zero quantity change for ProductVariant ID: {ProductVariantId}", request.ProductVariantId);
                throw new Exceptions.BadRequestException("Quantity change cannot be 0");
            }

            if (productVariant.StockQuantity + request.ChangeQuantity < 0)
            {
                _logger.LogWarning("Invalid quantity change would result in negative stock for ProductVariant ID: {ProductVariantId}", request.ProductVariantId);
                throw new Exceptions.BadRequestException("New stock quantity cannot be less than 0");
            }

            productVariant.StockQuantity += request.ChangeQuantity;
            productVariant.LastUpdatedAt = DateTime.UtcNow;
            productVariant.LastUpdatedBy = userId;

            await _productVariantRepository.UpdateAsync(productVariant, cancellationToken);

            var log = new InventoryLog()
            {
                ProductVariantId = productVariant.Id,
                ChangeQuantity = request.ChangeQuantity,
                NewStockQuantity = productVariant.StockQuantity,
                ChangeTypeId = (long)request.ChangeType,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            log = await _inventoryLogRepository.AddAsync(log, cancellationToken);

            _logger.LogInformation("Inventory log created for ProductVariant ID: {ProductVariantId} with ChangeQuantity: {ChangeQuantity} by User: {UserId}",
                productVariant.Id, request.ChangeQuantity, userId);

            var inventoryLogDTO = _mapper.Map<InventoryLogDTO>(log);
            inventoryLogDTO.ProductVariantSku = productVariant.Sku;
            inventoryLogDTO.ChangeType = ((InventoryLogChangeType)log.ChangeTypeId).ToString();

            return inventoryLogDTO;
        }
    }
}
