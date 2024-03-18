using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class OrderLine
    {
        public int Id { get; set; }
        public int? ProductConfigurationId { get; set; }
        public int? OrderId { get; set; }
        public int? Qty { get; set; }
        public int? Price { get; set; }

        public virtual ShopOrder? Order { get; set; }
        public virtual ProductConfiguration? ProductConfiguration { get; set; }

    }
}
