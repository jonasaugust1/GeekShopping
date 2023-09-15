using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model.Context;

namespace GeekShopping.CartAPI.Repository
{
    public class CardRepository : ICartRepository
    {
        private readonly MySQLContext _context;
        private IMapper _mapper;

        public CardRepository(MySQLContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<bool> ApplyCoupon(string userID, string couponCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ClearCart(string userID)
        {
            throw new NotImplementedException();
        }

        public Task<CartVO> FindCartByUserID(string userID)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveCoupon(string userID)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveFromCart(long cartDetailsId)
        {
            throw new NotImplementedException();
        }

        public Task<CartVO> SaveOrUpdateCart(CartVO cart)
        {
            throw new NotImplementedException();
        }
    }
}
