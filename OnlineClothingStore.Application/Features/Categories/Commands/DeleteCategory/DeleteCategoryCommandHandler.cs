using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            ICurrentUserService currentUserService,
            ILogger<DeleteCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling DeleteCategoryCommand for category ID: {CategoryId} by User: {UserId}", request.Id, userId);

            var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingCategory is null)
            {
                _logger.LogWarning("Category not found with ID: {CategoryId}", request.Id);
                throw new Exceptions.NotFoundException("Category with this id doesn't exist");
            }

            var hasChildren = await _categoryRepository.HasChildrenAsync(request.Id, cancellationToken);
            if (hasChildren)
            {
                _logger.LogWarning("Cannot delete category ID: {CategoryId} because it has subcategories", request.Id);
                throw new Exceptions.ConflictException("Cannot delete category because it has subcategories");
            }

            var hasProducts = await _categoryRepository.HasProductsAsync(existingCategory.Id, cancellationToken);
            if (hasProducts)
            {
                _logger.LogWarning("Cannot delete category ID: {CategoryId} because it has active products", request.Id);
                throw new Exceptions.ConflictException("Cannot delete category because it has active products");
            }

            await _categoryRepository.DeleteAsync(existingCategory, cancellationToken);

            _logger.LogInformation("Category with ID: {CategoryId} deleted successfully by User: {UserId}", request.Id, userId);
        }

    }
}
