using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model;

namespace GeekShopping.CartAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartVO> FindCartByUserID(string userID);
        Task<CartVO> SaveOrUpdateCart(CartVO cart);
        Task Save(Cart cart);
        Task Update(Cart cart);
        Task<bool> RemoveFromCart(long cartDetailsId);
        Task<bool> ApplyCoupon(string userID, string couponCode);
        Task<bool> RemoveCoupon(string userID);
        Task<bool> ClearCart(string userID);


    }
}
