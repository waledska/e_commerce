using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class Promotion
    {
        public Promotion()
        {
            PromotionCategories = new HashSet<PromotionCategory>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? DiscountRate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ICollection<PromotionCategory> PromotionCategories { get; set; }
    }
}
