using AutoMapper;
using ProductsApi.Data.Entities;
using ProductsApi.Models;

namespace ProductsApi.Infrastructure
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductModel>();
            CreateMap<ProductModel, Product>();
            CreateMap<Product, ProductTrimmed>();
        }
    }
}
