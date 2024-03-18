using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class OrderStatus
    {
        public OrderStatus()
        {
            ShopOrders = new HashSet<ShopOrder>();
        }

        public int Id { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<ShopOrder> ShopOrders { get; set; }
    }
}
