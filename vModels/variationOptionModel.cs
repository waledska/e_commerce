using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class variationOptionModel
    {
        public int Id { get; set; }
        [Required]
        public int? VariationId { get; set; }
        [Required]
        public string? Value { get; set; }
    }
}
