using FluentValidation;

namespace OnlineClothingStore.Application.Features.Products.Commands.UpdateProductVariant
{
    public class UpdateProductVariantValidator : AbstractValidator<UpdateProductVariantCommand>
    {
        public UpdateProductVariantValidator()
        {
            RuleFor(pv => pv.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than 0.");

            RuleFor(pv => pv.Size)
                .NotEmpty().WithMessage("Size is required.")
                .MaximumLength(20).WithMessage("Size must not exceed 20 characters.");

            RuleFor(pv => pv.Color)
                .NotEmpty().WithMessage("Color is required.")
                .MaximumLength(30).WithMessage("Color must not exceed 30 characters.");

            RuleFor(pv => pv.Sku)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

            RuleFor(pv => pv.StockQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock quantity cannot be negative.");

            RuleFor(pv => pv.ImageUrl)
                .MaximumLength(300).WithMessage("Image URL must not exceed 300 characters.")
                .When(pv => !string.IsNullOrWhiteSpace(pv.ImageUrl));
        }
    }
}
