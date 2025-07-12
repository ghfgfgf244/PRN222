using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace Web_.Pages.Patients
{
    public class IndexModel : PageModel
    {
        private readonly BusinessObjects.AppointmentsDbContext _context;

        public IndexModel(BusinessObjects.AppointmentsDbContext context)
        {
            _context = context;
        }

        public IList<Patient> Patient { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Patient = await _context.Patients
                .Include(p => p.RegisteredByNavigation).ToListAsync();
        }
    }
}
