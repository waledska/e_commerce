using e_commerce.EmailForms;
using e_commerce.Helpers;
using e_commerce.IdentityData;
using e_commerce.IdentityData.Models;
using e_commerce.vModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace e_commerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IEmailSender _emailSender;
        private readonly ISMSService _smsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IUrlHelper _urlHelper;
        //private readonly IUrlHelperFactory _urlHelperFactory;
        //private readonly IActionContextAccessor _actionContextAccessor;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt, IEmailSender emailSender,
            ISMSService smsService,
            IHttpContextAccessor httpContextAccessor)
            //IUrlHelper urlHelper,
            //IUrlHelperFactory urlHelperFactory, 
            //IActionContextAccessor actionContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _emailSender = emailSender;
            _smsService = smsService;
            _httpContextAccessor = httpContextAccessor;
            //_urlHelper = urlHelper;
            //_urlHelperFactory = urlHelperFactory;
            //_actionContextAccessor = actionContextAccessor;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };
            var userByPhone = await _userManager.Users.Where(p => p.PhoneNumber == model.PhoneNumber).FirstOrDefaultAsync();
            if (userByPhone is not null)
                return new AuthModel { Message = "Phone Number is already registered!" };

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Token = "",
                resetPassOTP = ""

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");
            await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Email = user.Email,
                PhoneNumber = model.PhoneNumber,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            bool isRightPass = !await _userManager.CheckPasswordAsync(user, model.Password);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;
        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }
        public async Task<string> RemoveRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (!await _userManager.IsInRoleAsync(user, model.Role))
                return "User already isn't assigned to this role";

            var result = await _userManager.RemoveFromRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<userRoles> getUserRolesAsync(string UserId)
        {
            var result = new userRoles();
            if (String.IsNullOrEmpty(UserId))
            {
                result.message = "user Id is required";
                return result;
            }
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                result.message = "user Id is wrong!";
                return result;
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || !roles.Any())
            {
                result.message = "this user doesn't have any roles";
                return result;
            }
            foreach (var rolename in roles)
            {
                var role = await _roleManager.FindByNameAsync(rolename);
                if (role != null)
                {
                    result.userInRoles.Add(new role { roleId = role.Id, roleName = rolename });
                }
            }
            result.userId = UserId;

            return result;
        }
        // find user by phone number or Gmail
        public async Task<ApplicationUser?> GetUserByPhoneOrMailAsync(string GmailOrPhone)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(GmailOrPhone);
            if (user != null)
            {
                return user;
            }

            // Assuming phone numbers are unique and properly formatted
            user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == GmailOrPhone);
            return user;
        }
        private string GenerateOtp(int length = 6)
        {
            var random = new Random();
            return new string(Enumerable.Repeat("0123456789", length)
                                          .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<resetPassTokenResult> sendToUserOTPAsync(resetPassTokenModel model)
        {
            resetPassTokenResult result = new resetPassTokenResult();
            bool isByPhone = true;
            var user = await _userManager.Users.FirstOrDefaultAsync(n => n.PhoneNumber == model.gmailOrPhone);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.gmailOrPhone);
                if (user != null)
                {
                    isByPhone = false;
                }
            }
            if (user == null)
            {
                return new resetPassTokenResult { message = "the Phone number or Gmail is wrong" };
            }

            // generate OTP and Token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var otp = GenerateOtp();
            var expiryTime = DateTime.UtcNow.AddMinutes(15);

            // store the new Token, OTP and the expire date in the DB
            if (user != null)
            {
                user.Token = token;
                user.resetPassOTP = otp;
                user.OtpExpiryTime = expiryTime;
                await _userManager.UpdateAsync(user);

            }

            // send the OTP to the user 
            if (isByPhone)
            {
                // send to Phone
                var userName = user.FirstName;
                var message = $"Hello {userName},\r\n\r\nYou recently requested to reset your password for your E_Commerce account. Your One-Time Password (OTP) is: {otp}. Please use this OTP to reset your password. If you didn't make this request, you can ignore this message.\r\n\r\nThank you,\r\nE_Commerce Team";
                var smsResult = _smsService.Send("+2" + model.gmailOrPhone, message);
                if (smsResult.ErrorMessage != null)
                {
                    return new resetPassTokenResult { message = smsResult.ErrorMessage };
                }
            }
            else
            {
                // send to Gmail
                GenerateMail mailGenerator = new GenerateMail();
                var mail = mailGenerator.ResetPassOTP(otp);
                await _emailSender.SendEmailAsync(model.gmailOrPhone, "E_Commerce", mail);
            }
            result.OTP = otp;
            return result;
        }
        private bool IsValidOtp(string providedOtp, string storedOtp, DateTime expiryTime)
        {
            return providedOtp == storedOtp && DateTime.UtcNow <= expiryTime;
        }

        public async Task<string> resetPassAsync(resetPassModel model)
        {
            string message = string.Empty;
            var user = await GetUserByPhoneOrMailAsync(model.gmailOrPhone);
            if (user == null)
            {
                return "invalid Gmail or Phone Number";
            }
            if (user.OtpExpiryTime == null)
            {
                return "Invalid or expired OTP.";
            }
            if (!IsValidOtp(model.OTP, user.resetPassOTP, (DateTime)user.OtpExpiryTime))
            {
                return "Invalid or expired OTP.";
            }
            if (model.newPassword != model.confirmNewPassword)
            {
                return "The new password and confirm new password fields do not match.";
            }

            // change the otp in the Db with random one for more security as we used it 
            user.resetPassOTP = GenerateOtp();
            await _userManager.UpdateAsync(user);

            var passwordChanged = await _userManager.ResetPasswordAsync(user, user.Token, model.newPassword);
            if (!passwordChanged.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in passwordChanged.Errors)
                {
                    errors.Add(error.Description);
                }
                message = string.Join(", ", errors);
                return message;
            }
            //in the api when implement make if message = "" api return => "Password reset successfully!"
            return message;
        }
        // 2 function to get user Id GetUserIdFromToken(helper), getUserId(main)
        public string getUserId()
        {
            // logic
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("HTTP context is not available.");
            }// Extract the Authorization header
            var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("Authorization header is missing or invalid.");
            }
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // Extract the user ID from the token
            return GetUserIdFromToken(token); // Reuse the previously defined methodreturn userId;
        }
        private string GetUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is required.", nameof(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "uid");

            if (userIdClaim != null)
            {
                return userIdClaim.Value;
            }
            else
            {
                throw new InvalidOperationException("User ID claim ('uid') not found in token.");
            }
        }
        public async Task<string> SendLinkConfirmGmailAsync(string userId)
        {
            string message = "";
            if (userId == null)
            {
                return "enter user's Id";
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "user not found";
            }
            var Gmail = await _userManager.GetEmailAsync(user);

            // create confirm email token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Dynamically construct the base URL from the request
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            // Build the confirmation link
            var apiUrl = "/api/Auth/confirmemail"; // Adjust the path as needed
            var confirmationLink = $"{baseUrl}{apiUrl}?userId={HttpUtility.UrlEncode(user.Id)}&token={HttpUtility.UrlEncode(token)}";

            // Build the confirmation link
            //var baseUrl = _httpContextAccessor.HttpContext.Request.GetDisplayUrl(); // Gets the base URL of the current request
            //var apiUrl = "/api/Auth/confirmemail"; // Path to your email confirmation endpoint
            //var confirmationLink = $"{baseUrl}{apiUrl}?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";

            // send the Email.
            var subject = "Confirm your email";
            var emailMessage = $"Please confirm your account by clicking this link: <a href=\"{confirmationLink}\">Confirm Email</a>";
            await _emailSender.SendEmailAsync(user.Email, subject, emailMessage);

            return message;
        }
        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return await _userManager.ConfirmEmailAsync(user, HttpUtility.UrlDecode(token));
        }
        public bool IsThereUserWithId(string userId)
        {
            var user = _userManager.GetUserId;
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
