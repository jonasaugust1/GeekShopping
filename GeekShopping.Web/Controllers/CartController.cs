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

        public CartController(
            IProductService productService,
            ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await FindUserCart());
        }

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

        private async Task<CartViewModel> FindUserCart()
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            string userId = GetUserId(token);

            CartViewModel? response = await _cartService.FindCartByUserId(userId, token);

            if (response?.CartHeader != null)
            {
                foreach (var detail in response.CartDetails)
                {
                    response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
                }
            }

            return response;
        }

        private string GetUserId(string? token)
        {
            Claim? userIdClaim = User.Claims.FirstOrDefault(u => u.Type == "sub");

            string userId = string.Empty;

            if (userIdClaim != null)
            {
                userId = userIdClaim.Value;
            }

            return userId;
        }
    }
}
