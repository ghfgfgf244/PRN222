using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.SignalR;
using Web_.Hubs;

namespace Web_.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserServices _context;
        private readonly IHubContext<AppHub> _hubContext;

        public IndexModel(IUserServices context, IHubContext<AppHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;

        }

        public IList<User> User { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? Role { get; set; }
        [BindProperty(SupportsGet = true)]
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
            await _hubContext.Clients.All.SendAsync("UserDeleted", id);

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
