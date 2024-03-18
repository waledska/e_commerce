using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class productItemUpdateModel
    {
        public productItemUpdateModel()
        {
            variationOptions_Ids = new int[0]; // Initializes with an empty array
        }

        public int Id { get; set; }
        [Required]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "Please enter a valid integer.")]
        public int? ProductId { get; set; }
        [Required, MaxLength(100)]
        public string? Sku { get; set; }
        [Required]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "Please enter a valid integer.")]
        public int? QtyInStock { get; set; }
        [DataType(DataType.Upload)]
        public List<IFormFile>? imageFormFiles { get; set; }
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Please enter a valid decimal.")]
        public decimal? Price { get; set; }
        [Required]
        public int[]? variationOptions_Ids { get; set; }
    }
}
