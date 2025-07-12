using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace Web_.Pages.Appointments
{
    public class IndexModel : PageModel
    {
        private readonly BusinessObjects.AppointmentsDbContext _context;

        public IndexModel(BusinessObjects.AppointmentsDbContext context)
        {
            _context = context;
        }

        public IList<Appointment> Appointment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Method)
                .Include(a => a.Patient)
                .Include(a => a.Slot)
                .Include(a => a.Specialty).ToListAsync();
        }
    }
}
