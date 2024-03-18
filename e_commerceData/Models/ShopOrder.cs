using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class ShopOrder
    {
        public ShopOrder()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? ShippingMethodId { get; set; }
        public int? OrderTotal { get; set; }
        public int? OrderStatusId { get; set; }

        public virtual OrderStatus? OrderStatus { get; set; }
        public virtual UserPaymentMethod? PaymentMethod { get; set; }
        public virtual Address? ShippingAddress { get; set; }
        public virtual ShippingMethod? ShippingMethod { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}
