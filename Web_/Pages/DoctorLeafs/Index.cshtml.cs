using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services;

namespace Web_.Pages.DoctorLeafs
{
    public class IndexModel : PageModel
    {
        private readonly IDoctorServices _doctorServices;
        private readonly IConfiguration _configuration;

        public IndexModel( IDoctorServices context, IConfiguration configuration)
        {
            _doctorServices = context;
            _configuration = configuration;
        }

        public IList<DoctorLeaf> DoctorLeafList { get; set; } = new List<DoctorLeaf>();
        public bool IsAdmin { get; set; }

        public async Task OnGetAsync()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
            var adminEmail = _configuration["DefaultAdmin:Username"];
            IsAdmin = userEmail == adminEmail;

            if (IsAdmin)
            {
                DoctorLeafList = await _doctorServices.GetAllDoctorLeavesAsync();
            }
            else
            {
                if (int.TryParse(userIdClaim, out int userId))
                {
                    DoctorLeafList = await _doctorServices.GetDoctorLeavesByDoctorIdAsync(userId);
                }
                else
                {
                    DoctorLeafList = new List<DoctorLeaf>(); 
                }
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _doctorServices.DeleteLeafAsync(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Date không tồn tại!";
            }
            else
            {
                TempData["SuccessMessage"] = "Date đã được xóa!";
            }
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostToggleStatusAsync(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var adminEmail = _configuration["DefaultAdmin:Username"];

            if (userEmail != adminEmail)
            {
                return new JsonResult(new { success = false, message = "Bạn không có quyền cập nhật!" });
            }

            var result = await _doctorServices.ToggleDoctorLeafStatusAsync(id);
            if (result == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy đơn nghỉ!" });
            }

            return new JsonResult(new
            {
                success = true,
                newValue = result
            });
        }
    }
}
