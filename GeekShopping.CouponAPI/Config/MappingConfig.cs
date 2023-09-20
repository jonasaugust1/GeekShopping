using AutoMapper;
using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Model;

namespace GeekShopping.CouponAPI.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration configuration = new(config =>
            {
                config.CreateMap<ProductVO, Product>().ReverseMap();
                config.CreateMap<CartVO, Cart>().ReverseMap();
                config.CreateMap<CartHeaderVO, CartHeader>().ReverseMap();
                config.CreateMap<CartDetailVO, CartDetail>().ReverseMap();
            });

            return configuration;
        }
    }
}
