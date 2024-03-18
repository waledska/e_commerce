using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace e_commerce.IdentityData.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        public string Token { get; set; }
        public string resetPassOTP { get; set; }
        public DateTime? OtpExpiryTime { get; set; }
    }
}
