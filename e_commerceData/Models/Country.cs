using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class Country
    {
        public Country()
        {
            Addresses = new HashSet<Address>();
        }

        public int Id { get; set; }
        public string? CountyName { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
    }
}
