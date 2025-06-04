using AutoMapper;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Application.Features.Brands.Commands.CreateBrand;
using OnlineClothingStore.Application.Features.Categories.Commands.CreateCategory;
using OnlineClothingStore.Application.Features.Products.Commands.CreateProduct;
using OnlineClothingStore.Application.Features.Products.Commands.CreateProductVariant;
using OnlineClothingStore.Application.Features.Products.Commands.UpdateProduct;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CreateCategoryCommand>().ReverseMap();

            CreateMap<Brand, BrandDTO>().ReverseMap();
            CreateMap<Brand, CreateBrandCommand>().ReverseMap();

            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, CreateProductCommand>().ReverseMap();
            CreateMap<Product, UpdateProductCommand>().ReverseMap();

            CreateMap<ProductVariant, ProductVariantDTO>().ReverseMap();
            CreateMap<ProductVariant, CreateProductVariantCommand>().ReverseMap();
        }
    }
}
