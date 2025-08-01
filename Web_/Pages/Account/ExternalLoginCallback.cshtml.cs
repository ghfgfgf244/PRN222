using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Web_.Pages.Account
{

    public class ExternalLoginCallbackModel : PageModel
    {
        private readonly IAccountServices _accountServices;
        private readonly IUserServices _userServices;
        private readonly IConfiguration _configuration;

        public ExternalLoginCallbackModel(IAccountServices context,IUserServices _context, IConfiguration configuration)
        {
            _accountServices = context;
            _userServices = _context;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Console.WriteLine("⏳ Bắt đầu callback từ Google");

            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Đăng nhập Google thất bại.";
                Console.WriteLine("❌ AuthenticateAsync thất bại.");
                return RedirectToPage("/Account/Login");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var avatarUrl = result.Principal.FindFirst("picture")?.Value;

            Console.WriteLine($"✅ Đã lấy thông tin từ Google - Email: {email}, Name: {name}, Avatar: {avatarUrl}");

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Không thể lấy email từ Google.";
                Console.WriteLine("❌ Email từ Google bị null hoặc rỗng.");
                return RedirectToPage("/Account/Login");
            }

            var user = await _accountServices.GetUserByEmailAsync(email);

            if (user == null)
            {
                Console.WriteLine("ℹ️ Người dùng chưa tồn tại. Tiến hành tạo mới...");

                user = new User
                {
                    Email = email,
                    FullName = name ?? "Người dùng mới",
                    Role = "Patient",
                    Avatar = avatarUrl,
                    IsActive = true,
                    Password = "GoogleUser",
                    Birthday = new DateTime(2000, 1, 1),
                    Gender = true, 
                    PhoneNumber = "0000000000",
                    CreatedAt = DateTime.Now
                };

                await _userServices.CreateUserAsync(user);

                Console.WriteLine($"✅ Tạo user thành công: {user.Email}, Id: {user.UserId}");
            }
            else
            {
                Console.WriteLine($"✅ Người dùng đã tồn tại: {user.Email}, Id: {user.UserId}");
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
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
                Console.WriteLine("ℹ️ Mật khẩu là GoogleUser (user mới tạo), lưu session GoogleUserId và chuyển hướng.");
                HttpContext.Session.SetInt32("GoogleUserId", user.UserId);

                // Đăng nhập luôn để tránh người dùng bị out
                var identityTemp = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principalTemp = new ClaimsPrincipal(identityTemp);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principalTemp);

                return RedirectToPage("/Appointments/Index");
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            Console.WriteLine($"✅ Đăng nhập thành công. Vai trò: {user.Role}");

            // Chuyển hướng theo Role
            return user.Role switch
            {
                "Doctor" => RedirectToPage("/Doctor/Dashboard"),
                "Patient" => RedirectToPage("/Appointments/Index"),
                _ => RedirectToPage("/Index"),
            };
        }

    }

}
