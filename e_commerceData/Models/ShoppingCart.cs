using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class ShoppingCart
    {
        public ShoppingCart()
        {
            ShoppingCartItems = new HashSet<ShoppingCartItem>();
        }

        public int Id { get; set; }
        public string? UserId { get; set; }

        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
