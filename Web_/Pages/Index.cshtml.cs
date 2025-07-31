using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using Services;

namespace Web_.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IExternalIntegrationService _externalIntegrationService;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _externalIntegrationService = new ExternalIntegrationService(configuration);
        }

        [BindProperty]
        public ContactInputModel ContactInfo { get; set; }
        public string? ResultMessage { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ResultMessage = "Vui lòng điền đầy đủ thông tin.";
                return Page();
            }

            try
            {
                string subject = $"{ContactInfo.Subject}";
                string body = $@"
<table style='width:100%; max-width:600px; margin:auto; border:1px solid #ddd; border-radius:10px; font-family:Segoe UI, sans-serif; background-color:#ffffff;'>
    <tr>
        <td style='background-color:#2c7be5; color:white; padding:20px; border-top-left-radius:10px; border-top-right-radius:10px;'>
            <h2 style='margin:0; font-size:20px;'>📩 Liên hệ từ website Health Care System</h2>
        </td>
    </tr>
    <tr>
        <td style='padding:24px 20px; font-size:15px; color:#333;'>
            <p style='margin:8px 0;'><strong>👤 Họ tên:</strong> {ContactInfo.Name}</p>
            <p style='margin:8px 0;'><strong>📧 Email:</strong> <a href='mailto:{ContactInfo.Email}' style='color:#2c7be5; text-decoration:none;'>{ContactInfo.Email}</a></p>
            <p style='margin:8px 0;'><strong>📝 Tiêu đề:</strong> {ContactInfo.Subject}</p>
            <p style='margin:16px 0 8px;'><strong>💬 Nội dung:</strong></p>
            <div style='padding:12px 16px; border-left:4px solid #2c7be5; background:#f4f8ff; border-radius:6px; white-space:pre-line;'>
                {ContactInfo.Message}
            </div>
        </td>
    </tr>
</table>";


                _externalIntegrationService.SendEmailAsync("hungghfgfgf244@gmail.com", subject, body, ContactInfo.Name, ContactInfo.Email);

                // ✅ Sau khi gửi thành công → reset form:
                ModelState.Clear(); // xoá trạng thái validation
                ContactInfo = new ContactInputModel(); // reset dữ liệu

                ResultMessage = "Gửi liên hệ thành công! Chúng tôi sẽ liên hệ bạn sớm nhất có thể. Xin cảm ơn!";
            }
            catch (Exception ex)
            {
                ResultMessage = $"Gửi thất bại: {ex.Message}";
            }

            return Page();
        }
        public class ContactInputModel()
        {
            [Required(ErrorMessage = "Họ và tên là bắt buộc")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Email là bắt buộc")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; }
            [Required(ErrorMessage = "Subject là bắt buộc")]
            public string Subject { get; set; }
            [Required(ErrorMessage = "Message là bắt buộc")]
            public string Message { get; set; }

        }
    }
}
