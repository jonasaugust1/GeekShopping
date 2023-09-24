using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Model.Base
{
    public class MySQLContext : DbContext
    {
        public MySQLContext() { }
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) 
        {

        }

        public DbSet<Coupon> Coupons { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                Id = 1,
                CouponCode = "JONAS_10",
                DiscountPercent = 10
            });
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                Id = 2,
                CouponCode = "MICROSERVICE_15",
                DiscountPercent = 15
            });
        }
    }
}
