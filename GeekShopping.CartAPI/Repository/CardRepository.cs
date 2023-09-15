using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> ApplyCoupon(string userID, string couponCode)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ClearCart(string userID)
        {
            throw new NotImplementedException();
        }

        public async Task<CartVO> FindCartByUserID(string userID)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveCoupon(string userID)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveFromCart(long cartDetailsId)
        {
            throw new NotImplementedException();
        }

        private async Task Save(Cart cart)
        {
            cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
            cart.CartDetails.FirstOrDefault().Product = null;

            _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
            await _context.SaveChangesAsync();
        }

        private async Task Update(Cart cart, CartDetail cartDetail)
        {
            cart.CartDetails.FirstOrDefault().Product = null;
            cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
            cart.CartDetails.FirstOrDefault().Id = cartDetail.Id;
            cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetail.Id;

            _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());

            await _context.SaveChangesAsync();
        }

        public async Task<CartVO> SaveOrUpdateCart(CartVO cartVO)
        {
            Cart cart = _mapper.Map<Cart>(cartVO);

            Product? product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == cartVO.CartDetails.FirstOrDefault().ProductId);

            if (product == null)
            {
                _context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _context.SaveChangesAsync();
            }

            CartHeader? cartHeader = await _context.CartHeaders.AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

            if(cartHeader == null)
            {
                _context.CartHeaders.Add(cartHeader);
                await _context.SaveChangesAsync();

                await Save(cart);
            }
            else
            {
                CartDetail cartDetail = await _context.CartDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductId == cartVO.CartDetails
                    .FirstOrDefault().ProductId && 
                    p.CartHeaderId == cartHeader.Id);

                if (cartDetail == null)
                {
                    await Save(cart);
                }
                else
                {
                    await Update(cart, cartDetail);
                }
            }


            return _mapper.Map<CartVO>(cart); 
        }
    }
}
