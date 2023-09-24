using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IRabbitMQMessageSender _rabbitMQSender;

        public CartController(ICartRepository cartRepository, ICouponRepository couponRepository, IRabbitMQMessageSender rabbitMQSender)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
            _rabbitMQSender = rabbitMQSender ?? throw new ArgumentNullException(nameof(rabbitMQSender));
        }

        [HttpGet("find-cart/{id}")]
        [Authorize]
        public async Task<ActionResult<CartVO>> FindById(string id)
        {
            CartVO cart = await _cartRepository.FindCartByUserID(id);

            if (cart == null) return NotFound();

            return Ok(cart);
        }

        [HttpPost("add-cart")]
        [Authorize]
        public async Task<ActionResult<CartVO>> AddCart(CartVO cartVO)
        {
            CartVO cart = await _cartRepository.SaveOrUpdateCart(cartVO);

            if (cart == null) return NotFound();

            return CreatedAtAction(nameof(AddCart), cart);
        }

        [HttpPut("update-cart")]
        [Authorize]
        public async Task<ActionResult<CartVO>> UpdateCart(CartVO cartVO)
        {
            CartVO cart = await _cartRepository.SaveOrUpdateCart(cartVO);

            if (cart == null) return NotFound();

            return Ok(cart);
        }

        [HttpDelete("remove-cart/{id}")]
        [Authorize]
        public async Task<ActionResult> RemoveCart(long id)
        {
            bool status = await _cartRepository.RemoveFromCart(id);

            if (!status) return BadRequest();

            return NoContent();
        }

        [HttpPost("apply-coupon")]
        [Authorize]
        public async Task<ActionResult<CartVO>> ApplyCoupon(CartVO cartVO)
        {
            bool response = await _cartRepository.ApplyCoupon(
                cartVO.CartHeader.UserId, 
                cartVO.CartHeader.CouponCode);

            if (!response) return NotFound();

            return Ok(response);
        }
        
        [HttpDelete("remove-coupon/{userId}")]
        [Authorize]
        public async Task<ActionResult<CartVO>> RemoveCoupon(string userId)
        {
            bool response = await _cartRepository.RemoveCoupon(userId);

            if (!response) return NotFound();

            return Ok(response);
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<ActionResult<CheckoutHeaderVO>> Checkout(CheckoutHeaderVO checkoutHeader)
        {
            string? token = Request.Headers["Authorization"];
            token = token?.Replace("Bearer", "");

            if (checkoutHeader?.UserId == null) return BadRequest();

            CartVO cart = await _cartRepository.FindCartByUserID(checkoutHeader.UserId);

            if (cart == null) return NotFound();

            if(!string.IsNullOrEmpty(checkoutHeader.CouponCode))
            {
                CouponVO coupon = await _couponRepository
                    .GetCoupon(checkoutHeader.CouponCode, token);

                if (checkoutHeader.DiscountPercent != coupon.DiscountPercent)
                {
                    return StatusCode(412);
                }
            }

            checkoutHeader.CartDetails = cart.CartDetails;
            checkoutHeader.PurchaseDate = DateTime.Now;

            _rabbitMQSender.SendMessage(checkoutHeader, "checkout_queue");

            return Ok(checkoutHeader);
        }
    }
}