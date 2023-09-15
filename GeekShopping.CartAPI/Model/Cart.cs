namespace GeekShopping.CartAPI.Model
{
    public class Cart
    {
        public CartHeader CartHeader { get; set; }
        IEnumerable<CartDetail> CartDetails { get; set; }
    }
}
