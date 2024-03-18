using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductConfigurations = new HashSet<ProductConfiguration>();
            UserReviews = new HashSet<UserReview>();
        }

        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ProductImage { get; set; }

        public virtual ProductCategory? Category { get; set; }
        public virtual ICollection<ProductConfiguration> ProductConfigurations { get; set; }
        public virtual ICollection<UserReview> UserReviews { get; set; }
    }
}
