using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class Variation
    {
        public Variation()
        {
            VariationOptions = new HashSet<VariationOption>();
        }

        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string? Name { get; set; }

        public virtual ProductCategory? Category { get; set; }
        public virtual ICollection<VariationOption> VariationOptions { get; set; }
    }
}
