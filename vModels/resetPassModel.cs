using System.ComponentModel.DataAnnotations;

namespace e_commerce.vModels
{
    public class resetPassModel
    {
        [Required]
        public string gmailOrPhone { get; set; }
        [Required]
        public string OTP { get; set; }
        [Required]
        public string newPassword { get; set; }
        [Required]
        public string confirmNewPassword { get; set; }
    }
}
