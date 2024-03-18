using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class reviewModel
    {
        public int Id { get; set; }
        [Required]
        public string? UserId { get; set; }
        [Required]
        public int? ProductId { get; set; }
        [Required]
        public int? RatingValue { get; set; }
        public string? Comment { get; set; }
    }
}
