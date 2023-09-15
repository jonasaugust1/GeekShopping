using GeekShopping.CartAPI.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.CartAPI.Model
{
    [Table("cart_detail")]
    public class CartDetail : BaseEntity
    {
        public long CartHeaderId { get; set; }

        [ForeignKey(nameof(CartHeaderId))]
        public CartHeader CartHeader { get; set; }

        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Column("count")]
        public int Count { get; set; }
    }
}
