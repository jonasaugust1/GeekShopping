namespace GeekShopping.CartAPI.Data.ValueObjects
{
    public class CartVO
    {
        public CartHeaderVO CartHeader { get; set; }
        IEnumerable<CartDetailVO> CartDetails { get; set; }
    }
}
