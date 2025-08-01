using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Security.Claims;
using BusinessObjects;
using Microsoft.AspNetCore.Authentication.Google;

namespace Web_.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountServices _accountServices;
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public LoginModel(IAccountServices context, IConfiguration configuration)
        {
            _accountServices = context;
            _configuration = configuration;
        }
        public bool EmailVerified { get; set; }
        public void OnGet(bool? verified)
        {
            EmailVerified = verified == true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
                return Page();
            }

            // Kiểm tra tài khoản admin từ appsettings
            var adminEmail = _configuration["DefaultAdmin:Username"];
            var adminPassword = _configuration["DefaultAdmin:Password"];
            var adminRole = _configuration["DefaultAdmin:Role"] ?? "Admin";

            if (Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) && Password == adminPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Administrator"),
                    new Claim(ClaimTypes.Email, Email),
                    new Claim("Role", adminRole)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToPage("/Admin/Dashboard");
            }

            // Kiểm tra tài khoản trong DB
            var user = await _accountServices.AuthenticateUserAsync(Email, Password);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Tài khoản không tồn tại hoặc mật khẩu không đúng.";
                return Page();
            }

            if (!user.IsActive)
            {
                TempData["ErrorMessage"] = "Tài khoản chưa được xác thực qua email. Vui lòng kiểm tra hộp thư của bạn.";
                return Page();
            }

            var claimsDb = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Role", user.Role ?? "Patient")
            };

            var identityDb = new ClaimsIdentity(claimsDb, CookieAuthenticationDefaults.AuthenticationScheme);
            var principalDb = new ClaimsPrincipal(identityDb);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principalDb);

            // ✅ Chuyển hướng theo Role
            switch (user.Role)
            {
                case "Doctor":
                    return RedirectToPage("/Appointments/Index");
                case "Patient":
                    return RedirectToPage("/Appointments/Index");
                default:
                    return RedirectToPage("/Index");
            }
        }
        //public async Task<IActionResult> OnPostGoogleLoginAsync()
        //{
        //    var properties = new AuthenticationProperties
        //    {
        //        RedirectUri = Url.Page("/Account/ExternalLogin")
        //    };

        //    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //}

    }
}
