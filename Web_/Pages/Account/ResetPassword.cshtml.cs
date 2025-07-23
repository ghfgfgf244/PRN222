using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Web_.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IUserServices _userServices;

        public ResetPasswordModel(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }
        public int? UserId { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Token));
                var parts = decoded.Split(':');
                if (parts.Length != 2) throw new Exception();

                UserId = int.Parse(parts[0]);
            }
            catch
            {
                TempData["ErrorMessage"] = "Liên kết không hợp lệ.";
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu không khớp hoặc rỗng.";
                return Page();
            }

            try
            {
                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Token));
                var parts = decoded.Split(':');
                var userId = int.Parse(parts[0]);

                var user = await _userServices.GetUserByIdAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Tài khoản không tồn tại.";
                    return Page();
                }

                var success = await _userServices.UpdateUserPasswordAsync(user.UserId, NewPassword);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật mật khẩu.";
                    return Page();
                }

                TempData["SuccessMessage"] = "Mật khẩu đã được đặt lại.";
                return RedirectToPage("/Account/Login");
            }
            catch
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi.";
                return Page();
            }
        }
    }
}