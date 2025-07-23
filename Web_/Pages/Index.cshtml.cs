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
<table style='width:100%;max-width:600px;margin:auto;border:1px solid #eee;border-radius:10px;font-family:sans-serif;'>
    <tr>
        <td style='background:#4CAF50;color:white;padding:20px;border-top-left-radius:10px;border-top-right-radius:10px;'>
            <h2 style='margin:0;'>📩 Thông tin liên hệ từ website</h2>
        </td>
    </tr>
    <tr>
        <td style='padding:20px;'>
            <p><strong>👤 Họ tên:</strong> {ContactInfo.Name}</p>
            <p><strong>📧 Email:</strong> <a href='mailto:{ContactInfo.Email}'>{ContactInfo.Email}</a></p>
            <p><strong>📝 Tiêu đề:</strong> {ContactInfo.Subject}</p>
            <p><strong>💬 Nội dung:</strong></p>
            <div style='padding:10px;border-left:4px solid #4CAF50;background:#f9f9f9;margin-top:5px;white-space:pre-line'>
                {ContactInfo.Message}
            </div>
        </td>
    </tr>
   <tr>
        <td style='padding:20px;font-size:12px;color:#888;text-align:center;border-top:1px solid #eee;'>
            Bạn nhận được email này vì ai đó đã liên hệ qua biểu mẫu tại website.<br/>
            Nếu không phải bạn, vui lòng bỏ qua email này.
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
