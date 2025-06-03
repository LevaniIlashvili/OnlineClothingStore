using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Brands.Queries.GetBrands
{
    public class GetBrandsQuery : IRequest<List<BrandDTO>> { }
}
