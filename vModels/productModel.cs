using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class productModel
    {
        [Required]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "Please enter a valid integer.")]
        public int CategoryId { get; set; }
        [Required, MaxLength(100)]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [DataType(DataType.Upload)]
        [Required]
        public IFormFile? imageFormFile { get; set; }
    }
}
