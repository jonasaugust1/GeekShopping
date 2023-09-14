using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;
        public const string BasePath = "api/v1/product";

        public ProductService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<ProductModel>> FindAllProducts(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            HttpResponseMessage response = await _client.GetAsync(BasePath);

            return await response.ReadContentAs<IEnumerable<ProductModel>>();
        }

        public async Task<ProductModel> FindProductById(long id, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _client.GetAsync($"{BasePath}/{id}");

            return await response.ReadContentAs<ProductModel>();
        }

        public async Task<ProductModel> CreateProduct(ProductModel productModel, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _client.PostAsJson(BasePath, productModel);

            if(response.IsSuccessStatusCode)
            return await response.ReadContentAs<ProductModel>();

            throw new Exception("Something went wrong when calling the API");
        }

        public async Task<ProductModel> UpdateProduct(ProductModel productModel, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _client.PutAsJson(BasePath, productModel);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<ProductModel>();

            throw new Exception("Something went wrong when calling the API");
        }

        public async Task<bool> DeleteProductById(long id, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _client.DeleteAsync($"{BasePath}/{id}");

            if(response.IsSuccessStatusCode)
                return true;

            throw new Exception("Something went wrong when calling the API");
        }
    }
}
