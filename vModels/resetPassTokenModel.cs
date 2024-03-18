using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class resetPassTokenModel
    {
        [Required]
        public string gmailOrPhone { get; set; }
    }
}
