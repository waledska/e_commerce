using System;
using System.Collections.Generic;

namespace e_commerce.e_commerceData.Models
{
    public partial class UserReview
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? ProductId { get; set; }
        public int? RatingValue { get; set; }
        public string? Comment { get; set; }

        public virtual Product? Product { get; set; }
    }
}
