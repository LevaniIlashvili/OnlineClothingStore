﻿using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Products.Commands.UpdateProductVariant
{
    public class UpdateProductVariantCommandHandler : IRequestHandler<UpdateProductVariantCommand>
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateProductVariantCommandHandler(
            IProductVariantRepository productVariantRepository,
            IProductRepository productRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _productVariantRepository = productVariantRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var existingProductVariant = await _productVariantRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingProductVariant is null)
            {
                throw new Exceptions.NotFoundException("Product variant not found");
            }

            if (existingProductVariant.Sku != request.Sku)
            {
                var otherProductWithSameSku = await _productVariantRepository.GetBySkuAsync(request.Sku, cancellationToken);
                if (otherProductWithSameSku is not null && otherProductWithSameSku.Id != request.Id)
                {
                    throw new Exceptions.ConflictException("Another product with the same sku already exists.");
                }
            }

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product is null)
            {
                throw new Exceptions.NotFoundException("Product not found");
            }

            var updatedProductVariant = _mapper.Map(request, existingProductVariant);
            updatedProductVariant.LastUpdatedAt = DateTime.UtcNow;
            updatedProductVariant.LastUpdatedBy = userId;

            await _productVariantRepository.UpdateAsync(updatedProductVariant, cancellationToken);
            
        }
    }
}
