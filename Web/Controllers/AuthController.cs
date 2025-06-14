using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View("Login");
        }
    }
}
