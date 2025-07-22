using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Services;

namespace Web_.Pages.Appointments
{
    public class DetailsModel : PageModel
    {
        private readonly IAppointmentServices _context;

        public DetailsModel(BusinessObjects.AppointmentsDbContext context)
        {
            _context =  new AppointmentServices(context);
        }

        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.GetAppointmentByIdAsync(id.Value);
            if (appointment == null)
            {
                return NotFound();
            }
            else
            {
                Appointment = appointment;
            }
            return Page();
        }

        [Authorize(Policy = "DoctorOnly")]
        public async Task<IActionResult> OnGetUpdateStatus(int id, string status)
        {
            var appointment = await _context.GetAppointmentByIdAsync(id);
            if (appointment == null || appointment.Status != "Confirmed")
                return NotFound();

            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(doctorIdClaim, out var doctorId))
                return RedirectToPage("/Account/AccessDenied");

            if (appointment.DoctorId != doctorId)
                return RedirectToPage("/Account/AccessDenied");

            if (appointment.Status != "Confirmed")
                return BadRequest("Không thể cập nhật trạng thái.");

            await _context.UpdateAppointmentStatusAsync(doctorId, status);

            return RedirectToPage("Details", new { id });
        }
    }
}
