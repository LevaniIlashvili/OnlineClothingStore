using FluentValidation;

namespace OnlineClothingStore.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .MaximumLength(100)
                .WithMessage("Product name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.SkuPrefix)
                .NotEmpty()
                .WithMessage("SKU Prefix is required.")
                .MaximumLength(10)
                .WithMessage("SKU Prefix must not exceed 10 characters.");

            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId must be a valid ID.");

            RuleFor(x => x.BrandId).GreaterThan(0).WithMessage("BrandId must be a valid ID.");
        }
    }
}
