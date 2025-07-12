using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;

namespace Web_.Pages.Appointments
{
    public class CreateModel : PageModel
    {
        private readonly BusinessObjects.AppointmentsDbContext _context;

        public CreateModel(BusinessObjects.AppointmentsDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "FullName");
        ViewData["MethodId"] = new SelectList(_context.ExamMethods, "MethodId", "Name");
        ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
        ViewData["SlotId"] = new SelectList(_context.TimeSlots, "SlotId", "SlotId");
        ViewData["SpecialtyId"] = new SelectList(_context.DoctorSpecialties, "SpecialtyId", "Name");
            return Page();
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Appointments.Add(Appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
