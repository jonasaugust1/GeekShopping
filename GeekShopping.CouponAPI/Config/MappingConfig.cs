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
                config.CreateMap<CouponVO, Coupon>().ReverseMap();
            });

            return configuration;
        }
    }
}
