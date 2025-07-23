using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Web_.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IUserServices _userServices;
        private readonly IExternalIntegrationService _emailService;

        public ForgotPasswordModel(IUserServices userServices, IExternalIntegrationService emailService)
        {
            _userServices = userServices;
            _emailService = emailService;
        }

        [BindProperty]
        public string Email { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập email.";
                return Page();
            }

            var user = await _userServices.GetUserByEmailAsync(Email);
            if (user == null || !user.IsActive)
            {
                TempData["ErrorMessage"] = "Email không tồn tại hoặc chưa xác thực.";
                return Page();
            }

            var currentDomain = $"{Request.Scheme}://{Request.Host}";
            var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{user.UserId}:{Guid.NewGuid()}"));

            var resetLink = $"{currentDomain}/Account/ResetPassword?token={token}";

            var body = $"<p>Bạn đã yêu cầu đặt lại mật khẩu.</p><p><a href='{resetLink}'>Nhấn vào đây để đặt lại mật khẩu</a></p>";

            await _emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu", body, "YourApp Support");

            TempData["SuccessMessage"] = "Đã gửi email khôi phục mật khẩu. Vui lòng kiểm tra hộp thư.";
            return RedirectToPage("/Account/Login");
        }
    }
}