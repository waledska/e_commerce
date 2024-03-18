using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class PaymentType
    {
        public PaymentType()
        {
            UserPaymentMethods = new HashSet<UserPaymentMethod>();
        }

        public int Id { get; set; }
        public string Value { get; set; } = null!;
        public string? ImgUrl { get; set; }

        public virtual ICollection<UserPaymentMethod> UserPaymentMethods { get; set; }
    }
}
