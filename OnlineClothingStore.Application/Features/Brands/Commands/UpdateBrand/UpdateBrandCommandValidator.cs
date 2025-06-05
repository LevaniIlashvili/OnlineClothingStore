using FluentValidation;

namespace OnlineClothingStore.Application.Features.Brands.Commands.UpdateBrand
{
    public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
    {
        public UpdateBrandCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required")
                .MaximumLength(100).WithMessage("Name can't exceed 100 characters");
        }
    }
}
