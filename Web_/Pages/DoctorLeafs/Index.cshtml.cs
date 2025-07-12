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
    public class IndexModel : PageModel
    {
        private readonly BusinessObjects.AppointmentsDbContext _context;

        public IndexModel(BusinessObjects.AppointmentsDbContext context)
        {
            _context = context;
        }

        public IList<DoctorLeaf> DoctorLeaf { get;set; } = default!;

        public async Task OnGetAsync()
        {
            DoctorLeaf = await _context.DoctorLeaves
                .Include(d => d.Doctor).ToListAsync();
        }
    }
}
