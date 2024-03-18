using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class UserAddress
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? AddressId { get; set; }
        public bool? IsDefault { get; set; }

        public virtual Address? Address { get; set; }
    }
}
