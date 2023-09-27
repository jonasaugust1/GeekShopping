using GeekShopping.Email.Model;

namespace GeekShopping.Email.Repository
{
    public interface IOrderRepository
    {
        Task UpdateOrderStatus(long orderHeaderId, bool paid);
    }
}
