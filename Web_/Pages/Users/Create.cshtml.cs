using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;
using DataAccessObjects;
using Services;
using System.Reflection.Metadata;

namespace Web_.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly IUserServices _context;
        private readonly IExternalIntegrationService _exContext;
        private readonly IWebHostEnvironment _environment;


        public CreateModel(BusinessObjects.AppointmentsDbContext context, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _context = new UserServices(context);
            _exContext = new ExternalIntegrationService(configuration);
            this._environment = environment;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }
        [BindProperty]
        public IFormFile? Upload { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Lấy đường dẫn ảnh từ form (do js upload trước đó và gán vào input hidden)
            if (!string.IsNullOrEmpty(Request.Form["AvatarPath"]))
            {
                User.Avatar = Request.Form["AvatarPath"];
            }

            await _context.CreateUserAsync(User);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUploadAvatarAsync()
        {
            if (Upload == null || Upload.Length == 0)
                return BadRequest(new { success = false, error = "No file uploaded" });

            // Tạo tên file duy nhất
            string uniqueFileName = $"{Guid.NewGuid()}_{Upload.FileName}";
            string keyNameInBucket = $"avatars/{uniqueFileName}";

            // Đọc file từ Upload vào MemoryStream
            using var memoryStream = new MemoryStream();
            await Upload.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset stream về đầu

            // Gọi SupaBase upload
            var publicUrl = await _exContext.UploadImageStreamAsync(memoryStream, keyNameInBucket, Upload.ContentType);

            return new JsonResult(new { success = true, filePath = publicUrl });
        }
    }
}
