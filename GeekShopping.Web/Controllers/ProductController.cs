using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [Authorize]
        public async Task<IActionResult> ProductIndex()
        {
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            IEnumerable<ProductViewModel> products = await _productService.FindAllProducts(accessToken);

            return View(products);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductViewModel productModel)
        {
            if(ModelState.IsValid)
            {
                string? accessToken = await HttpContext.GetTokenAsync("access_token");

                ProductViewModel response = await _productService.CreateProduct(productModel, accessToken);

                if(response != null) return RedirectToAction(nameof(ProductIndex));
            }

            return View(productModel);
        }

        public async Task<IActionResult> ProductUpdate(long id)
        {
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            ProductViewModel product = await _productService.FindProductById(id, accessToken);
            if(product != null) return View(product);

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductUpdate(ProductViewModel productModel)
        {
            if (ModelState.IsValid)
            {
                string? accessToken = await HttpContext.GetTokenAsync("access_token");

                ProductViewModel response = await _productService.UpdateProduct(productModel, accessToken);

                if (response != null) return RedirectToAction(nameof(ProductIndex));
            }

            return View(productModel);
        }

        [Authorize]
        public async Task<IActionResult> ProductDelete(long id)
        {
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            ProductViewModel product = await _productService.FindProductById(id, accessToken);
            if (product != null) return View(product);

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> ProductDelete(ProductViewModel productModel)
        {
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            bool deleted = await _productService.DeleteProductById(productModel.Id, accessToken);

            if (deleted) return RedirectToAction(nameof(ProductIndex));

            return View(productModel);
        }
    }
}
