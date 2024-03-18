using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class addVariationForProductItemsModel
    {
        public addVariationForProductItemsModel()
        {
            itemVariationOptions = new List<itemVariationOptionModel>();
        }

        [Required]
        public int ProductId { get; set; }
        [Required]
        public int variationId { get; set; }
        [Required]
        public List<itemVariationOptionModel>? itemVariationOptions { get; set; }

    }
}
