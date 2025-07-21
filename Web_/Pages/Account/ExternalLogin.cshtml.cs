using BusinessObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web_.Pages.Account
{
    public class ExternalLoginModel : PageModel
    {
        private readonly AppointmentsDbContext _context;

        public ExternalLoginModel(AppointmentsDbContext context)
        {
            _context = context;
        }
        public IActionResult OnGet(string provider)
        {
            Console.WriteLine($"🔁 Bắt đầu đăng nhập bằng {provider}");

            var redirectUrl = Url.Page("/Account/ExternalLoginCallback", pageHandler: null, values: null, protocol: Request.Scheme);
            Console.WriteLine($"➡️ Redirect URL: {redirectUrl}");

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

    }
}
