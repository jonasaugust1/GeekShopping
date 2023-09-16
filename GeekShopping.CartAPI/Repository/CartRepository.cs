using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly MySQLContext _context;
        private IMapper _mapper;

        public CartRepository(MySQLContext context, IMapper mapper)
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
            CartHeader cartHeader = await _context.CartHeaders
                       .FirstOrDefaultAsync(c => c.UserId == userID);

            if(cartHeader != null)
            {
                _context.CartDetails
                    .RemoveRange(
                    _context.CartDetails
                    .Where(c => c.CartHeaderId == cartHeader.Id));

                _context.CartHeaders.Remove(cartHeader);

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<CartVO> FindCartByUserID(string userID)
        {
            Cart cart = new()
            {
                CartHeader = await _context.CartHeaders
                .FirstOrDefaultAsync(c => c.UserId == userID),
            };

            cart.CartDetails = _context.CartDetails
                .Where(c => c.CartHeaderId == cart.CartHeader.Id)
                .Include(c => c.Product);

            return _mapper.Map<CartVO>(cart);
        }

        public async Task<bool> RemoveCoupon(string userID)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveFromCart(long cartDetailsId)
        {
            try
            {
                CartDetail cartDetail = await _context.CartDetails
                    .FirstOrDefaultAsync(c => c.Id == cartDetailsId);

                int total = _context.CartDetails.Where(
                    c => c.CartHeaderId == cartDetail.CartHeaderId)
                    .Count();

                _context.CartDetails.Remove(cartDetail);

                if(total == 1)
                {
                    CartHeader cartHeaderToRemove = await _context.CartHeaders
                        .FirstOrDefaultAsync(c => c.Id == cartDetail.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private async Task Save(Cart cart, CartHeader cartHeader = null)
        {
            long cartHeaderID = cartHeader != null ? cartHeader.Id : cart.CartHeader.Id;

            cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderID;
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

            //Checks if the product is already saved in database if not then save
            Product? product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == cart.CartDetails.FirstOrDefault().ProductId);

            if (product == null)
            {
                _context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _context.SaveChangesAsync();
            }

            CartHeader? cartHeader = await _context.CartHeaders.AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

            if(cartHeader == null)
            {
                //Create CartHeader and CartDetails
                _context.CartHeaders.Add(cartHeader);
                await _context.SaveChangesAsync();

                await Save(cart);
            }
            else
            {
                //Checks if CartDetails has same product
                CartDetail cartDetail = await _context.CartDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductId == cart.CartDetails
                    .FirstOrDefault().ProductId && 
                    p.CartHeaderId == cartHeader.Id);

                if (cartDetail == null)
                {
                    Save(cart, cartHeader);
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
