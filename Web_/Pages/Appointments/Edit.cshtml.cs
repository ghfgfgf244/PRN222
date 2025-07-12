using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace Web_.Pages.Appointments
{
    public class EditModel : PageModel
    {
        private readonly BusinessObjects.AppointmentsDbContext _context;

        public EditModel(BusinessObjects.AppointmentsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment =  await _context.Appointments.FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            Appointment = appointment;
           ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "FullName");
           ViewData["MethodId"] = new SelectList(_context.ExamMethods, "MethodId", "Name");
           ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
           ViewData["SlotId"] = new SelectList(_context.TimeSlots, "SlotId", "SlotId");
           ViewData["SpecialtyId"] = new SelectList(_context.DoctorSpecialties, "SpecialtyId", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(Appointment.AppointmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
