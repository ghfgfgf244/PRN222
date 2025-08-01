using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace Web_.Pages.Account
{
    public class VerifyEmailModel : PageModel
    {
        private readonly IUserServices _context;

        public VerifyEmailModel(IUserServices context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Page();
            }

            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var parts = decoded.Split(':');
            var userId = int.Parse(parts[0]);

            var user = await _context.GetUserByIdAsync(userId);
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
