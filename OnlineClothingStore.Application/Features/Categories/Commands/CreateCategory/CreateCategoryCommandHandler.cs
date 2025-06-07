using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDTO>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<CreateCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryDTO> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling CreateCategoryCommand for name: {CategoryName} by User: {UserId}", request.Name, userId);

            var normalizedName = request.Name.Trim().ToLower();

            var existingCategory = await _categoryRepository.GetByNameAsync(normalizedName, cancellationToken);
            if (existingCategory is not null)
            {
                _logger.LogWarning("Category creation failed: Category with name '{CategoryName}' already exists", normalizedName);
                throw new Exceptions.ConflictException("Category with same name already exists");
            }

            if (request.ParentCategoryId is not null)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                if (parentCategory is null)
                {
                    _logger.LogWarning("Parent category not found with ID: {ParentCategoryId}", request.ParentCategoryId);
                    throw new Exceptions.NotFoundException("Parent category not found");
                }
            }

            var category = _mapper.Map<Category>(request);
            category.CreatedAt = DateTime.UtcNow;
            category.CreatedBy = userId;
            category.Name = normalizedName;

            var addedCategory = await _categoryRepository.AddAsync(category, cancellationToken);

            _logger.LogInformation("Category '{CategoryName}' created successfully by User: {UserId}", normalizedName, userId);
            return _mapper.Map<CategoryDTO>(addedCategory);
        }

    }
}
