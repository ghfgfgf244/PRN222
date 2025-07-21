using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Services;

namespace Web_.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserServices _context;

        public IndexModel(BusinessObjects.AppointmentsDbContext context)
        {
            _context = new UserServices(context);
        }

        public IList<User> User { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? Role { get; set; }

        public int TotalPages { get; set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            var (users, totalPages) = await _context.GetPagedUsersAsync(Role, SearchTerm, pageNumber, pageSize);
            User = users;
            TotalPages = totalPages;
            CurrentPage = pageNumber;
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _context.DeleteUserAsync(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "User không tồn tại!";
            }
            else
            {
                TempData["SuccessMessage"] = "User đã được xóa!";
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(int id)
        {
            var newStatus = await _context.ToggleUserStatusAsync(id);

            if (newStatus == null)
            {
                return NotFound();
            }
            return new JsonResult(new
            {
                success = true,
                newValue = newStatus
            });
        }
    }
}
