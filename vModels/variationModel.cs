using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class variationModel
    {
        public int Id { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
