using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class UserAddressModel
    {
        [Required, MaxLength(100)]
        public string unitNumber { get; set; }
        [Required, MaxLength(100)]
        public string streetNumber { get; set; }
        [Required, MaxLength(200)]
        public string city { get; set; }
        [MaxLength(300)]
        public string region { get; set; }
        [Required, MaxLength(100)]
        public string postalCode { get; set; }
        [Required]
        public int countryId { get; set; }
        public UserAddressModel()
        {
            region = string.Empty;
        }
    }
}
