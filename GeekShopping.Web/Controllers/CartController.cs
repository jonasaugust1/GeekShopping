using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
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

        public async Task<IActionResult> CartIndex()
        {
            return View(await FindUserCart());
        }

        private async Task<CartViewModel> FindUserCart()
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            Claim? userIdClaim = User.Claims.FirstOrDefault(u => u.Type == "sub");

            string userId = string.Empty;

            if (userIdClaim != null)
            {
                userId = userIdClaim.Value;
            }

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
    }
}
