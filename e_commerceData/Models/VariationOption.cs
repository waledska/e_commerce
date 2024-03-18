using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class VariationOption
    {

        public int Id { get; set; }
        public int? VariationId { get; set; }
        public string? Value { get; set; }

        public virtual Variation? Variation { get; set; }
    }
}
