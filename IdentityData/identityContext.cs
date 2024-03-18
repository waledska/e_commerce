using e_commerce.IdentityData.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace e_commerce.IdentityData
{
    public class identityContext: IdentityDbContext<ApplicationUser>
    {
        public identityContext(DbContextOptions<identityContext> options) : base(options)
        {
                
        }
    }
}
