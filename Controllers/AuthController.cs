using e_commerce.Services;
using e_commerce.vModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace e_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _authService = authService;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        // sign in/up APIs
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(
                new
                {
                    isAuthenticated = result.IsAuthenticated,
                    username = result.Username,
                    email = result.Email,
                    PhoneNumber = result.PhoneNumber,
                    roles = result.Roles,
                    token = result.Token,
                    expiresOn = result.ExpiresOn
                });
        }
        [HttpPost("login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(
            new
            {
                isAuthenticated = result.IsAuthenticated,
                username = result.Username,
                email = result.Email,
                roles = result.Roles,
                token = result.Token,
                expiresOn = result.ExpiresOn
            });
        }
        // Roles APIs
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost("addToRole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost("removeFromRole")]
        public async Task<IActionResult> removeRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RemoveRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("getAllRoles")]
        public async Task<IActionResult> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var result = roles.Select(x => new { role = x.Name, id = x.Id}).ToList();
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("getUserRoles/{userId}")]
        public async Task<IActionResult> getUserRolesAsync(string userId)
        {
            var result = await _authService.getUserRolesAsync(userId);
            if (!string.IsNullOrEmpty(result.message))
                return BadRequest(new { Message = result.message });
            return Ok(new { userId = result.userId, userRoles = result.userInRoles });
        }
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")] // you forget password already how you will send Token
        [HttpPost("sendOtpToResetPass")]
        public async Task<IActionResult> getTokenForResetPass([FromBody] resetPassTokenModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.sendToUserOTPAsync(model);
            if (!string.IsNullOrEmpty(result.message))
                return (BadRequest(new {message = result.message }));
            // when the token is generated correctly
            return Ok(new {message = "OTP send to user successfully" });
        }
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")] // you forget password already how you will send Token
        [HttpPost("resetPass")]
        public async Task<IActionResult> resetPass(resetPassModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.resetPassAsync(model);
            if(!string.IsNullOrEmpty(result))
                return BadRequest((new {message = result}));
            return Ok(new { message = "Password reset successfully!" });
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("getUserIdFromToken")]
        public async Task<IActionResult> getUserId()
        {
            return Ok(_authService.getUserId());
        }
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("sendConfirmationEmail")]
        public async Task<IActionResult> snedConfirmationEmail(string userId)
        {
            var message = await _authService.SendLinkConfirmGmailAsync(userId);
            if(message == null)
            {
                return Ok("Confirmation Link send send to user's Email Successfully");
            }
            else
            {
                return BadRequest(message);
            }
        }
        // this API the user will request it by clicking on the link that he recieved 
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("User ID and token are required.");
            }

            var result = await _authService.ConfirmEmailAsync(userId, token);
            if (result.Succeeded)
            {
                return Ok("Email successfully confirmed.");
            }
            else
            {
                return BadRequest("Error confirming your email.");
            }
        }
        [HttpGet("sendmail")]
        public async Task<IActionResult> sendmail()
        {
            await _emailSender.SendEmailAsync("waledmohamed360@gmail.com", "yaaaaaa", "some how some waaay");
            return Ok("send!");
        }

    }
}
