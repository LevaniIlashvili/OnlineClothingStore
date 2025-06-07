using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            ICurrentUserService currentUserService,
            ILogger<UpdateCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling UpdateCategoryCommand for Category ID: {CategoryId} by User: {UserId}", request.Id, userId);

            var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingCategory is null)
            {
                _logger.LogWarning("Category not found with ID: {CategoryId}", request.Id);
                throw new Exceptions.NotFoundException("Category with this id not found");
            }

            var normalizedName = request.Name.Trim().ToLower();
            var categoryWithThisName = await _categoryRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (categoryWithThisName is not null && request.Id != categoryWithThisName.Id)
            {
                _logger.LogWarning("Conflict: Another category with name '{CategoryName}' already exists (ID: {ConflictId})",
                    normalizedName, categoryWithThisName.Id);
                throw new Exceptions.ConflictException("Category with this name already exists");
            }

            if (request.ParentCategoryId is not null)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                if (parentCategory is null)
                {
                    _logger.LogWarning("Parent category not found with ID: {ParentCategoryId}", request.ParentCategoryId);
                    throw new Exceptions.NotFoundException("Parent category with this id not found");
                }
            }

            existingCategory.Name = normalizedName;
            existingCategory.ParentCategoryId = request.ParentCategoryId;
            existingCategory.LastUpdatedAt = DateTime.UtcNow;
            existingCategory.LastUpdatedBy = userId;

            await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);

            _logger.LogInformation("Category with ID: {CategoryId} updated successfully by User: {UserId}", request.Id, userId);
        }

    }
}
