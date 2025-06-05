using FluentValidation;

namespace OnlineClothingStore.Application.Features.Carts.Commands.AddToCart
{
    public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.ProductVariantId)
                .GreaterThan(0)
                .WithMessage("ProductVariantId must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be at least 1.");
        }
    }
}
