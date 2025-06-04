using FluentValidation;

namespace OnlineClothingStore.Application.Features.Carts.Commands.UpdateCartItem
{
    public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
    {
        public UpdateCartItemCommandValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.CartItemId)
                .GreaterThan(0)
                .WithMessage("CartItemId must be greater than 0.");

            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
        }
    }
}
