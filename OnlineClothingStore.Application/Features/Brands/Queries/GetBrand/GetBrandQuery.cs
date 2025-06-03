using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Brands.Queries.GetBrand
{
    public class GetBrandQuery : IRequest<BrandDTO>
    {
        public long Id { get; set; }
    }
}
