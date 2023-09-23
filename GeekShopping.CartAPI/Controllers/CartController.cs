using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repository;

        public CartController(ICartRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("find-cart/{id}")]
        [Authorize]
        public async Task<ActionResult<CartVO>> FindById(string id)
        {
            CartVO cart = await _repository.FindCartByUserID(id);

            if (cart == null) return NotFound();

            return Ok(cart);
        }

        [HttpPost("add-cart")]
        [Authorize]
        public async Task<ActionResult<CartVO>> AddCart(CartVO cartVO)
        {
            CartVO cart = await _repository.SaveOrUpdateCart(cartVO);

            if (cart == null) return NotFound();

            return CreatedAtAction(nameof(AddCart), cart);
        }

        [HttpPut("update-cart")]
        [Authorize]
        public async Task<ActionResult<CartVO>> UpdateCart(CartVO cartVO)
        {
            CartVO cart = await _repository.SaveOrUpdateCart(cartVO);

            if (cart == null) return NotFound();

            return Ok(cart);
        }

        [HttpDelete("remove-cart/{id}")]
        [Authorize]
        public async Task<ActionResult> RemoveCart(long id)
        {
            bool status = await _repository.RemoveFromCart(id);

            if (!status) return BadRequest();

            return NoContent();
        }

        [HttpPost("apply-coupon")]
        [Authorize]
        public async Task<ActionResult<CartVO>> ApplyCoupon(CartVO cartVO)
        {
            bool response = await _repository.ApplyCoupon(
                cartVO.CartHeader.UserId, 
                cartVO.CartHeader.CouponCode);

            if (!response) return NotFound();

            return Ok(response);
        }
        
        [HttpDelete("remove-coupon/{userId}")]
        [Authorize]
        public async Task<ActionResult<CartVO>> RemoveCoupon(string userId)
        {
            bool response = await _repository.RemoveCoupon(userId);

            if (!response) return NotFound();

            return Ok(response);
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<ActionResult<CheckoutHeaderVO>> Checkout(CheckoutHeaderVO checkoutHeader)
        {
            if(checkoutHeader?.UserId == null) return BadRequest();

            CartVO cart = await _repository.FindCartByUserID(checkoutHeader.UserId);

            if (cart == null) return NotFound();

            checkoutHeader.CartDetails = cart.CartDetails;
            checkoutHeader.DateTime = DateTime.Now;
            //RabbitMQ comes here

            return Ok(checkoutHeader);
        }
    }
}