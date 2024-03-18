using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class UserPaymentMethod
    {
        public UserPaymentMethod()
        {
            ShopOrders = new HashSet<ShopOrder>();
        }

        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? PaymentTypeId { get; set; }
        public string? Provider { get; set; }
        public string? AccountNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public bool? IsDefault { get; set; }

        public virtual PaymentType? PaymentType { get; set; }
        public virtual ICollection<ShopOrder> ShopOrders { get; set; }
    }
}
