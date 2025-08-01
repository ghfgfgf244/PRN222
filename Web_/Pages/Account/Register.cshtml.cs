using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.ComponentModel.DataAnnotations;

namespace Web_.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IUserServices _userServices;
        private readonly IExternalIntegrationService _externalIntegrationService;

        public RegisterModel(IUserServices context, IConfiguration configuration)
        {
            _userServices = context;
            _externalIntegrationService = new ExternalIntegrationService(configuration);
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class RegisterInput
        {
            [Required]
            public string FullName { get; set; }

            [Required]
            [Phone]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }

            public bool Gender { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime Birthday { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (await _userServices.EmailExistsAsync(Input.Email))
            {
                ErrorMessage = "Email đã được sử dụng.";
                return Page();
            }

            if (await _userServices.PhoneExistsAsync(Input.PhoneNumber))
            {
                ErrorMessage = "Số điện thoại đã được sử dụng.";
                return Page();
            }

            var user = new User
            {
                FullName = Input.FullName,
                PhoneNumber = Input.PhoneNumber,
                Email = Input.Email,
                Password = Input.Password,
                Gender = Input.Gender,
                Birthday = Input.Birthday,
                Role = "Patient",
                Avatar = null,
                CreatedAt = DateTime.Now,
                IsActive = false
            };

            await _userServices.CreateUserAsync(user);

            var currentDomain = $"{Request.Scheme}://{Request.Host}";
            var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{user.UserId}:{Guid.NewGuid()}"));

            var verifyLink = $"{currentDomain}/Account/VerifyEmail?token={token}";
            var subject = "Xác thực tài khoản";
            var body = $"<p>Nhấn vào liên kết sau để kích hoạt tài khoản:</p><a href='{verifyLink}'>Xác thực Email</a>";

            await _externalIntegrationService.SendEmailAsync(user.Email,subject,body,"System");

            // Optional: Gửi email xác thực ở đây

            return RedirectToPage("/Account/Login");
        }
    }

}
