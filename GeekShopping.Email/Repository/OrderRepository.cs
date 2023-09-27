using GeekShopping.Email.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<MySQLContext> _context;

        public OrderRepository(DbContextOptions<MySQLContext> context)
        {
            _context = context;
        }

        public async Task UpdateOrderStatus(long orderHeaderId, bool status)
        {
            //await using MySQLContext _db = new(_context);

            //OrderHeader? header = await _db.OrderHeaders
            //    .FirstOrDefaultAsync(o => o.Id == orderHeaderId);

            //if (header != null)
            //{
            //    header.PaymentStatus = status;

            //    _db.OrderHeaders.Update(header);

            //    await _db.SaveChangesAsync();
            //}
        }
    }
}
