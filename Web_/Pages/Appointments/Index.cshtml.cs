using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Services;
using System.Data;

namespace Web_.Pages.Appointments
{
    public class IndexModel : PageModel
    {
        private readonly IAppointmentServices _context;

        public IndexModel(IAppointmentServices context)
        {
            _context = context;
        }
        public string CurrentDomain { get; set; }

        public IList<Appointment> Appointment { get; set; } = default!;
        public bool IsDoctor { get; set; }
        public bool IsPatient { get; set; }
        [BindProperty(SupportsGet = true)]
        public int TotalPages { get; set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        public async Task<IActionResult> OnGetAsync(string? searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            SearchTerm = searchTerm;
            CurrentDomain = $"{Request.Scheme}://{Request.Host}";

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst("Role")?.Value;
            IsDoctor = role == "Doctor";
            IsPatient = role == "Patient";

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(role) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            IQueryable<Appointment> query;

            if (IsPatient)
            {
                query = _context.GetAppointmentsByPatientQuery(userId);
            }
            else if (IsDoctor)
            {
                query = _context.GetAppointmentsByDoctorQuery(userId);
            }
            else
            {
                Appointment = new List<Appointment>();
                TotalPages = 0;
                CurrentPage = pageNumber;
                return Page(); 
            }

            var (appointments, totalPages) = await _context.GetPagedAppointmentsAsync(query, searchTerm, pageNumber, pageSize);
            Appointment = appointments;
            TotalPages = totalPages;
            CurrentPage = pageNumber;

            return Page();
        }


        public async Task<IActionResult> OnPostCancelledAsync(int id)
        {
            Console.WriteLine(">>> Gọi OnPostCancelledAsync với id = " + id);

            var result = await _context.CancelAppointmentAsync(id);
            Console.WriteLine(">>> Kết quả cancel: " + result); if (!result)
            {
                TempData["ErrorMessage"] = "Lịch khám không tồn tại!";
            }
            else
            {
                TempData["SuccessMessage"] = "Đã hủy lịch khám!";
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Account/AccessDenied");
            }
            Appointment = await _context.GetAppointmentsByRegisteredByAsync(userId);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst("Role")?.Value;

            if (role != "Doctor" || !int.TryParse(doctorIdClaim, out var doctorId))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            var appointment = await _context.GetAppointmentByIdAsync(id);
            if (appointment == null || appointment.DoctorId != doctorId)
                return new JsonResult(new { success = false, message = "Lịch khám không tồn tại hoặc bạn không có quyền." });

            appointment.Status = status;
            await _context.UpdateAppointmentStatusAsync(id, status);
            return new JsonResult(new { success = true, message = "Cập nhật trạng thái thành công!" });
        }

    }
}
