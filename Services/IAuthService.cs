using e_commerce.vModels;
using Microsoft.AspNetCore.Identity;

namespace e_commerce.Services
{
    public interface IAuthService
    {
        string getUserId();
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<string> RemoveRoleAsync(AddRoleModel model);
        Task<userRoles> getUserRolesAsync(string UserId);
        Task<resetPassTokenResult> sendToUserOTPAsync(resetPassTokenModel model);
        Task<string> resetPassAsync(resetPassModel model);
        Task<string> SendLinkConfirmGmailAsync(string userId);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        public bool IsThereUserWithId(string userId);
    }
}
