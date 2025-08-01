using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Services;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Web_.Hubs;

namespace Web_.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserServices _context;
        private readonly IExternalIntegrationService _exContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IDoctorServices _doctorContext;
        private readonly IHubContext<AppHub> _hubContext;

        public EditModel(IUserServices context, IDoctorServices _contextDoctor, IConfiguration configuration, IWebHostEnvironment environment, IHubContext<AppHub> hubContext)
        {
            _context = context;
            _exContext = new ExternalIntegrationService(configuration);
            this._environment = environment;
            _configuration = configuration;
            _doctorContext = _contextDoctor;
            _hubContext = hubContext;

        }

        [BindProperty]
        public User UserEdit { get; set; } = default!;
        public IFormFile? Upload { get; set; }
        public bool IsAdmin { get; set; }
        [BindProperty]
        public int? SelectedSpecialtyId { get; set; }
        public List<SelectListItem> SpecialtyOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var adminEmail = _configuration["DefaultAdmin:Username"];
            IsAdmin = userEmail == adminEmail;
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }
            if (!IsAdmin && user.UserId.ToString() != userIdClaim)
                return RedirectToPage("/Account/AccessDenied");
            UserEdit = user;

            SpecialtyOptions = (await _doctorContext.GetAllSpecialties())
                .Select(s => new SelectListItem
                {
                    Value = s.SpecialtyId.ToString(),
                    Text = s.Name
                })
                .ToList();

            // Lấy SpecialtyId nếu là Doctor
            if (user.Role == "Doctor")
            {
                var specialtyId = await _doctorContext.GetSpecialtyIdByDoctorId(user.UserId);
                SelectedSpecialtyId = specialtyId;
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var adminEmail = _configuration["DefaultAdmin:Username"];
            IsAdmin = userEmail == adminEmail;

            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (!IsAdmin && UserEdit.UserId.ToString() != userIdClaim)
                return RedirectToPage("/Account/AccessDenied");
            if (!string.IsNullOrEmpty(Request.Form["AvatarPath"]))
            {
                UserEdit.Avatar = Request.Form["AvatarPath"];
            }

            await _context.UpdateUserAsync(UserEdit);

            if (UserEdit.Role == "Doctor" && SelectedSpecialtyId.HasValue)
            {
                await _doctorContext.UpdateDoctorSpecialtyAsync(UserEdit.UserId, SelectedSpecialtyId.Value);
            }

            if (!IsAdmin)
            {
                TempData["SuccessMessage"] = "Thông tin đã được cập nhật.";
                return Page(); 
            }
            await _hubContext.Clients.All.SendAsync("LoadUsers");

            return RedirectToPage("./Index", new { role = UserEdit.Role });
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
