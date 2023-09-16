using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeekShopping.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(
            ILogger<HomeController> logger, 
            IProductService productService,
            ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<ProductViewModel> products = await _productService.FindAllProducts("");

            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(long id)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            ProductViewModel product = await _productService.FindProductById(id, token);

            return View(product);
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductViewModel product)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            CartViewModel cart = new()
            {
                CartHeader = new CartHeaderViewModel
                {
                    UserId = User.Claims
                    .Where(u => u.Type == "sub")?
                    .FirstOrDefault()?.Value,
                },
            };

            CartDetailViewModel cartDetail = new CartDetailViewModel
            {
                Count = product.Count,
                ProductId = product.Id,
                Product = await _productService.FindProductById(product.Id, token),
            };

            List<CartDetailViewModel> cartDetails = new();

            cartDetails.Add(cartDetail);

            CartViewModel? response = await _cartService.AddItemToCart(cart, token);

            if(response == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            string? accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}