using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace Web_.Pages.DoctorLeafs
{
    public class EditModel : PageModel
    {
        private readonly BusinessObjects.AppointmentsDbContext _context;

        public EditModel(BusinessObjects.AppointmentsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DoctorLeaf DoctorLeaf { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorleaf =  await _context.DoctorLeaves.FirstOrDefaultAsync(m => m.LeaveId == id);
            if (doctorleaf == null)
            {
                return NotFound();
            }
            DoctorLeaf = doctorleaf;
           ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "FullName");
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

            _context.Attach(DoctorLeaf).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorLeafExists(DoctorLeaf.LeaveId))
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

        private bool DoctorLeafExists(int id)
        {
            return _context.DoctorLeaves.Any(e => e.LeaveId == id);
        }
    }
}
