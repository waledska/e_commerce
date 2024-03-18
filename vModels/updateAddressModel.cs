using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class updateAddressModel
    {
        [Required]
        public int addressId { get; set; }
        [Required]
        public UserAddressModel newAddressData { get; set; }
    }
}
