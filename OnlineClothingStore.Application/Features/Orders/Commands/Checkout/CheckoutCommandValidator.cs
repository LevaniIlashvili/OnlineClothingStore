using FluentValidation;

namespace OnlineClothingStore.Application.Features.Orders.Commands.Checkout
{
    public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
    {
        public CheckoutCommandValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .WithMessage("Shipping address is required")
                .MaximumLength(500)
                .WithMessage("Shipping address must not exceed 500 characters");
        }
    }
}
