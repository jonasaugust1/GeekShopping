﻿using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly MySQLContext _context;
        private readonly IMapper _mapper;

        public CartRepository(MySQLContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCoupon(string userID, string couponCode)
        {
            CartHeader? cartHeader = await _context.CartHeaders
                       .FirstOrDefaultAsync(c => c.UserId == userID);

            if (cartHeader != null)
            {
                cartHeader.CouponCode = couponCode;

                _context.CartHeaders.Update(cartHeader);

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> RemoveCoupon(string userID)
        {
            CartHeader? cartHeader = await _context.CartHeaders
                      .FirstOrDefaultAsync(c => c.UserId == userID);

            if (cartHeader != null)
            {
                cartHeader.CouponCode = null;

                _context.CartHeaders.Update(cartHeader);

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> ClearCart(string userID)
        {
            CartHeader? cartHeader = await _context.CartHeaders
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
            try
            {
                CartHeader? cartHeader = await _context.CartHeaders
                .FirstOrDefaultAsync(c => c.UserId == userID);

                Cart cart = new()
                {
                    CartHeader = cartHeader,
                };

                cart.CartDetails = await _context.CartDetails
                    .Where(c => c.CartHeaderId == cart.CartHeader.Id)
                    .Include(c => c.Product)
                    .ToListAsync();

                return _mapper.Map<CartVO>(cart);
            } 
            catch (Exception ex)
            {
                return new CartVO();
            }
        }

        public async Task<bool> RemoveFromCart(long cartDetailsId)
        {
            try
            {
                CartDetail? cartDetail = await _context.CartDetails
                    .FirstOrDefaultAsync(c => c.Id == cartDetailsId);

                int total = _context.CartDetails.Where(
                    c => c.CartHeaderId == cartDetail.CartHeaderId)
                    .Count();

                _context.CartDetails.Remove(cartDetail);

                if(total == 1)
                {
                    CartHeader? cartHeaderToRemove = await _context.CartHeaders
                        .FirstOrDefaultAsync(c => c.Id == cartDetail.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
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
                _context.CartHeaders.Add(cart.CartHeader);
                await _context.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;
                _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
            else
            {
                //Checks if CartDetails has same product
                CartDetail? cartDetail = await _context.CartDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductId == cart.CartDetails
                    .FirstOrDefault().ProductId && 
                    p.CartHeaderId == cartHeader.Id);

                if (cartDetail == null)
                {
                    //Create CartDetails
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //Update product count and CartDetails
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
                    cart.CartDetails.FirstOrDefault().Id = cartDetail.Id;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetail.CartHeaderId;
                    _context.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _context.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartVO>(cart); 
        }
    }
}
