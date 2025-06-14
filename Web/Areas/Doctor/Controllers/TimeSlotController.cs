using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class TimeSlotController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
