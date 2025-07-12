using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;

namespace Web_.Pages.DoctorLeafs
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
            return Page();
        }

        [BindProperty]
        public DoctorLeaf DoctorLeaf { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DoctorLeaves.Add(DoctorLeaf);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
