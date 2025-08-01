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
using Microsoft.AspNetCore.SignalR;
using Web_.Hubs;

namespace Web_.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly IUserServices _context;
        private readonly IExternalIntegrationService _exContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IDoctorServices _doctorContext;
        private readonly IHubContext<AppHub> _hubContext;

        public CreateModel(IUserServices context,IDoctorServices _contextDoctor, IConfiguration configuration, IWebHostEnvironment environment, IHubContext<AppHub> hubContext)
        {
            _context = context;
            _exContext = new ExternalIntegrationService(configuration);
            this._environment = environment;
            _doctorContext = _contextDoctor;
            _hubContext = hubContext;
        }

        [BindProperty]
        public User User { get; set; } = default!;
        [BindProperty]
        public int? SelectedSpecialtyId { get; set; }
        public List<SelectListItem> SpecialtyOptions { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            SpecialtyOptions = (await _doctorContext.GetAllSpecialties())
                .Select(s => new SelectListItem
                {
                    Value = s.SpecialtyId.ToString(),
                    Text = s.Name
                })
                .ToList();

            return Page();
        }

        [BindProperty]
        public IFormFile? Upload { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                SpecialtyOptions = (await _doctorContext.GetAllSpecialties())
                   .Select(s => new SelectListItem
                   {
                       Value = s.SpecialtyId.ToString(),
                       Text = s.Name
                   })
                   .ToList();

                return Page();
            }

            // Lấy đường dẫn ảnh từ form (do js upload trước đó và gán vào input hidden)
            if (!string.IsNullOrEmpty(Request.Form["AvatarPath"]))
            {
                User.Avatar = Request.Form["AvatarPath"];
            }
            User.CreatedAt = DateTime.Now;
            await _context.CreateUserAsync(User);

            if (User.Role == "Doctor" && SelectedSpecialtyId.HasValue)
            {
                await _doctorContext.AddSpecialtyToDoctorAsync(User.UserId, SelectedSpecialtyId.Value);
            }
            await _hubContext.Clients.All.SendAsync("LoadUsers");

            return RedirectToPage("./Index", new { role = User.Role });
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
