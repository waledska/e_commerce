using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_commerce.e_commerceData.Models
{
    public partial class ShoppingCartItem
    {
        public int Id { get; set; }
        //[ForeignKey("ShopingCartId")]----------loook
        public int? CartId { get; set; }
        public int? ProductConfigurationId { get; set; }
        public int? Qty { get; set; }

        public virtual ShoppingCart? Cart { get; set; }
        public virtual ProductConfiguration? ProductConfiguration { get; set; }

        //-------------- under
        //public virtual Product Product { get; set; } // Navigation property to Product entity
        //public virtual ICollection<ProductItemImag> ProductItemImags { get; set; } // Navigation property to ProductItemImag entity
        //public virtual ICollection<ProductConfiguration> ProductConfigurations { get; set; } // Navigation property to ProductConfiguration entity
        //public virtual ICollection<OrderLine> OrderLines { get; set; } // Navigation property to OrderLine entity
    }
}
