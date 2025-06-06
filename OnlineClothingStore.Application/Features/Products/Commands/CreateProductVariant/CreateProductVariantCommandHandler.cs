﻿using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Products.Commands.CreateProductVariant
{
    public class CreateProductVariantCommandHandler : IRequestHandler<CreateProductVariantCommand, ProductVariantDTO>
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateProductVariantCommandHandler(
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

        public async Task<ProductVariantDTO> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product is null)
            {
                throw new Exceptions.NotFoundException("Product not found");
            }

            var variantWithThisSku = await _productVariantRepository.GetBySkuAsync(request.Sku, cancellationToken);

            if (variantWithThisSku is not null)
            {
                throw new Exceptions.ConflictException("Product variant with this sku already exists");
            }

            var variantToAdd = _mapper.Map<ProductVariant>(request);
            variantToAdd.CreatedAt = DateTime.UtcNow;
            variantToAdd.CreatedBy = userId;
            var addedVariant = await _productVariantRepository.AddAsync(variantToAdd, cancellationToken);

            return _mapper.Map<ProductVariantDTO>(addedVariant);
        }
    }
}
