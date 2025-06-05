using MediatR;

namespace OnlineClothingStore.Application.Features.Brands.Commands.DeleteBrand
{
    public class DeleteBrandCommand : IRequest
    {
        public long Id { get; set; }
    }
}
