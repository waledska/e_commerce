using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class ProductConfiguration
    {
        // configure lists of objects
        public ProductConfiguration()
        {
            ProductConfigurationPhotos = new HashSet<ProductConfigurationPhoto>();
            ShoppingCartItems = new HashSet<ShoppingCartItem>();
            OrderLines = new HashSet<OrderLine>();
        }

        // sttributes
        public int Id { get; set; }
        public int? Product_Id { get; set; }
        public string? VariationOptionIds { get; set; }

        public string? SKU { get; set; }
        public int? QtyInStock { get; set; }
        public decimal? Price { get; set; }

        // relations
        public virtual Product? product { get; set; }

        public virtual ICollection<OrderLine> OrderLines { get; set; }
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

        public virtual ICollection<ProductConfigurationPhoto> ProductConfigurationPhotos { get; set; }
        
    }
}
