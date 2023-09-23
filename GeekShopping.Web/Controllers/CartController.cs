using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GeekShopping.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(
            IProductService productService,
            ICartService cartService,
            ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await FindCartByUserId());
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        [Authorize]
        public async Task<IActionResult> ApplyCoupon(CartViewModel cart)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            bool response = await _cartService.ApplyCoupon(cart, token);

            if (response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }
        
        [HttpPost]
        [ActionName("RemoveCoupon")]
        [Authorize]
        public async Task<IActionResult> RemoveCoupon()
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            string userId = GetUserId();

            bool response = await _cartService.RemoveCoupon(userId, token);

            if (response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Remove(long id)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            bool response = await _cartService.RemoveFromCart(id, token);

            if (response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await FindCartByUserId());
        }

        private async Task<CartViewModel> FindCartByUserId()
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            string userId = GetUserId();

            CartViewModel? response = await _cartService.FindCartByUserId(userId, token);

            if (response?.CartHeader != null)
            {
                await ValidateCoupon(response, token);

                foreach (var detail in response.CartDetails)
                {
                    response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
                }

                SetDiscount(response);
            }

            return response;
        }

        private string GetUserId()
        {
            Claim? userIdClaim = User.Claims.FirstOrDefault(u => u.Type == "sub");

            string userId = string.Empty;

            if (userIdClaim != null)
            {
                userId = userIdClaim.Value;
            }

            return userId;
        }

        private async Task<bool> ValidateCoupon(CartViewModel cart, string token)
        {
            if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                CouponViewModel? coupon = await _couponService
                    .GetCoupon(cart.CartHeader.CouponCode, token);

                if (coupon?.CouponCode != null)
                {
                    cart.CartHeader.DiscountAmount = coupon.DiscountAmount;

                    return true;
                }
            }

            return false;
        }

        private static void SetDiscount(CartViewModel cart)
        {
            decimal discountPercent = cart.CartHeader.DiscountAmount / 100;
            decimal discountAmount = cart.CartHeader.PurchaseAmount * discountPercent;

            cart.CartHeader.DiscountAmount = discountAmount;
            cart.CartHeader.PurchaseAmount = cart.CartHeader.PurchaseAmount - discountAmount;
        }
    }
}
