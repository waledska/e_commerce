using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            InverseParentCategory = new HashSet<ProductCategory>();
            Products = new HashSet<Product>();
            PromotionCategories = new HashSet<PromotionCategory>();
            Variations = new HashSet<Variation>();
        }

        public int Id { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImgUrl { get; set; }

        public virtual ProductCategory? ParentCategory { get; set; }
        public virtual ICollection<ProductCategory> InverseParentCategory { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<PromotionCategory> PromotionCategories { get; set; }
        public virtual ICollection<Variation> Variations { get; set; }
    }
}
