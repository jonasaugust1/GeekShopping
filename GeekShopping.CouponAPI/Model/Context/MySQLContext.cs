using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Model.Base
{
    public class MySQLContext : DbContext
    {
        public MySQLContext() { }
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) 
        {

        }

        public DbSet<Product> Products { get; set;}
    }
}
