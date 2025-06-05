using FluentValidation;

namespace OnlineClothingStore.Application.Features.Brands.Commands.CreateBrand
{
    public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
    {
        public CreateBrandCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required")
                .MaximumLength(100).WithMessage("Name can't exceed 100 characters");
        }
    }
}
