using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace Web_.Pages.DoctorLeafs
{
    public class DetailsModel : PageModel
    {
        private readonly BusinessObjects.AppointmentsDbContext _context;

        public DetailsModel(BusinessObjects.AppointmentsDbContext context)
        {
            _context = context;
        }

        public DoctorLeaf DoctorLeaf { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorleaf = await _context.DoctorLeaves.FirstOrDefaultAsync(m => m.LeaveId == id);
            if (doctorleaf == null)
            {
                return NotFound();
            }
            else
            {
                DoctorLeaf = doctorleaf;
            }
            return Page();
        }
    }
}
