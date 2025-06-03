using MediatR;

namespace OnlineClothingStore.Application.Features.Brands.Commands.UpdateBrand
{
    public class UpdateBrandCommand : IRequest
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
