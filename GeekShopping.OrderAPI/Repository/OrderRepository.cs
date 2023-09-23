using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Model.Context;
using GeekShopping.OrderAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<MySQLContext> _context;

        public OrderRepository(DbContextOptions<MySQLContext> context)
        {
            _context = context;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            if(orderHeader == null) return false;

            await using MySQLContext _db = new(_context); 

            _db.OrderHeaders.Add(orderHeader);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task UpdateOrderStatus(long orderHeaderId, bool status)
        {
            await using MySQLContext _db = new(_context);

            OrderHeader? header = await _db.OrderHeaders
                .FirstOrDefaultAsync(o => o.Id == orderHeaderId);

            if (header != null)
            {
                header.PaymentStatus = status;

                _db.OrderHeaders.Update(header);

                await _db.SaveChangesAsync();
            }
        }
    }
}
