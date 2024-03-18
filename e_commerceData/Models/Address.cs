using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class Address
    {
        public Address()
        {
            ShopOrders = new HashSet<ShopOrder>();
            UserAddresses = new HashSet<UserAddress>();
        }

        public int Id { get; set; }
        public string? UnitNumber { get; set; }
        public string? StreetNumber { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }

        public virtual Country? Country { get; set; }
        public virtual ICollection<ShopOrder> ShopOrders { get; set; }
        public virtual ICollection<UserAddress> UserAddresses { get; set; }
    }
}
