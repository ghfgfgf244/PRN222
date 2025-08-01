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

        public DetailsModel(IAppointmentServices context)
        {
            _context = context;
        }

        public Appointment Appointment { get; set; } = default!;
        public bool IsDoctor { get; set; }
        public bool IsPatient { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role = User.FindFirst("Role")?.Value;
            IsDoctor = role == "Doctor";
            IsPatient = role == "Patient";

            var appointment = await _context.GetAppointmentByIdAsync(id.Value);
            if (appointment == null)
            {
                return NotFound();
            }

            Appointment = appointment;

            if (IsDoctor)
            {
                if (appointment.Status != "Confirmed")
                {
                    await _context.UpdateAppointmentStatusAsync(id.Value, "Confirmed");

                    Appointment = await _context.GetAppointmentByIdAsync(id.Value);
                }
            }
            return Page();
        }
    }
}
