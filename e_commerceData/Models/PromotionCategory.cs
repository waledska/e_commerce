using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class PromotionCategory
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? PromotionId { get; set; }

        public virtual ProductCategory? Category { get; set; }
        public virtual Promotion? Promotion { get; set; }
    }
}
