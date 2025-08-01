using System.Security.Claims;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Web_.Pages.DoctorLeafs
{
    public class CreateModel : PageModel
    {
        private readonly IDoctorServices _doctorService;

        public CreateModel(IDoctorServices context)
        {
            _doctorService = context;
        }

        [BindProperty]
        public DoctorLeaf DoctorLeaf { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page(); 
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int doctorId))
            {
                ModelState.AddModelError("", "Không thể xác định danh tính người dùng. Vui lòng đăng nhập lại.");
                return Page();
            }

            DoctorLeaf.DoctorId = doctorId;
            DoctorLeaf.CreatedAt = DateTime.Now;
            DoctorLeaf.IsActive = false;

            var success = await _doctorService.RegisterDoctorLeaveAsync(DoctorLeaf);

            if (!success)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var minDate = today.AddDays(7);

                if (DoctorLeaf.LeaveDate < minDate)
                {
                    ModelState.AddModelError("", $"Ngày nghỉ phải được đăng ký trước ít nhất 7 ngày (sớm nhất là {minDate:dd/MM/yyyy}).");
                }
                else
                {
                    ModelState.AddModelError("", "Bạn đã đăng ký nghỉ vào ngày này rồi.");
                }

                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}
