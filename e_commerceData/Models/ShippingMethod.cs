using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class ShippingMethod
    {
        public ShippingMethod()
        {
            ShopOrders = new HashSet<ShopOrder>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Price { get; set; }
        public string? ImgUrl { get; set; }

        public virtual ICollection<ShopOrder> ShopOrders { get; set; }
    }
}
