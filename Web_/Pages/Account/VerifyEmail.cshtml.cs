using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Web_.Pages.Account
{
    public class VerifyEmailModel : PageModel
    {
        private readonly IUserServices _context;

        public VerifyEmailModel(AppointmentsDbContext context)
        {
            _context = new UserServices(context);
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _context.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!user.IsActive)
            {
                await _context.VerifyEmailAsync(user.UserId);
            }
            TempData["VerifySuccess"] = true;
            return RedirectToPage("/Account/Login", new { verified = true });
        }
    }

}
