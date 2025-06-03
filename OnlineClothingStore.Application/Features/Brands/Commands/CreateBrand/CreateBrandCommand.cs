using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Brands.Commands.CreateBrand
{
    public class CreateBrandCommand : IRequest<BrandDTO>
    {
        public string Name { get; set; } = null!;
    }
}
