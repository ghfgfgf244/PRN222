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

            var body = $@"
<div style='font-family:Segoe UI, Arial, sans-serif; font-size:16px; color:#333; line-height:1.6; max-width:600px; margin:auto; background-color:#f9f9f9; padding:32px 16px;'>
    <div style='background-color:#ffffff; padding:24px 32px; border:1px solid #ddd; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,0.05);'>
        
        <h2 style='color:#2c7be5; margin-top:0;'>🔐 Yêu cầu đặt lại mật khẩu</h2>

        <p>Xin chào <strong>{user.FullName}</strong>,</p>

        <p>Bạn đã gửi yêu cầu <strong>đặt lại mật khẩu</strong> cho tài khoản của mình tại <strong>Health Care System</strong>.</p>

        <p>Vui lòng nhấn vào nút bên dưới để tiếp tục quá trình:</p>

        <div style='text-align:center; margin:30px 0;'>
            <a href='{resetLink}' style='display:inline-block; background-color:#2c7be5; color:#ffffff; padding:12px 28px; font-size:16px; font-weight:bold; border-radius:6px; text-decoration:none;'>
                Đặt lại mật khẩu
            </a>
        </div>

        <p style='color:#555;'>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>

        <hr style='margin:40px 0; border:none; border-top:1px solid #eee;' />

        <p style='font-size:13px; color:#999; text-align:center;'>
            © {DateTime.Now.Year} Health Care System. Email này được gửi tự động, vui lòng không trả lời lại.
        </p>
    </div>
</div>";



            await _emailService.SendEmailAsync(
                user.Email,
                "Yêu cầu đặt lại mật khẩu - Health Care System",
                body,
                "Health Care System Hỗ trợ <no-reply@healthcaresystem.com>"
            );

            TempData["SuccessMessage"] = "Đã gửi email khôi phục mật khẩu. Vui lòng kiểm tra hộp thư.";
            return RedirectToPage("/Account/Login");
        }
    }
}