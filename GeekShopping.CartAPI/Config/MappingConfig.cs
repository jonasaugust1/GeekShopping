using AutoMapper;
using GeekShopping.CartAPI.Model;

namespace GeekShopping.CartAPI.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration configuration = new(config =>
            {
                //config.CreateMap<ProductVO, Product>();
                //config.CreateMap<Product, ProductVO>();
            });

            return configuration;
        }
    }
}
