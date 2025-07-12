using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Identity;

namespace Web_.Pages.Account
{
    public class ExternalLoginCallbackModel : PageModel
    {
        private readonly IAccountServices _accountServices;
        private readonly IUserServices _userServices;
        private readonly IConfiguration _configuration;

        public ExternalLoginCallbackModel(AppointmentsDbContext context, IConfiguration configuration)
        {
            _accountServices = new AccountServices(context);
            _userServices = new UserServices(context);
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Đăng nhập Google thất bại.";
                return RedirectToPage("/Account/Login");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var avatarUrl = result.Principal.FindFirst("picture")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Không thể lấy email từ Google.";
                return RedirectToPage("/Account/Login");
            }

            var user = await _accountServices.GetUserByEmailAsync(email);

            // Nếu user chưa tồn tại → tạo mới
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FullName = name ?? "Người dùng mới",
                    Role = "Patient", // mặc định
                    Avatar = avatarUrl,
                    IsActive = true,
                    Password = "GoogleUser"
                };

                await _userServices.CreateUserAsync(user);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Role", user.Role ?? "Patient")
            };

            if (!string.IsNullOrEmpty(user.Avatar))
            {
                claims.Add(new Claim("Avatar", user.Avatar));
            }

            var hasher = new PasswordHasher<User>();
            var isTempPassword = hasher.VerifyHashedPassword(user, user.Password, "GoogleUser") == PasswordVerificationResult.Success;

            if (isTempPassword)
            {
                HttpContext.Session.SetInt32("GoogleUserId", user.UserId);
                return RedirectToPage("/Patient/CompleteProfile");
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // ✅ Chuyển hướng theo Role
            switch (user.Role)
            {
                case "Doctor":
                    return RedirectToPage("/Doctor/Dashboard");
                case "Patient":
                    return RedirectToPage("/Patient/Home");
                default:
                    return RedirectToPage("/Index");
            }
        }
    }

}
