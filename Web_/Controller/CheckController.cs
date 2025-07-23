using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Web_.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public CheckController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { exists = false });

            var exists = await _userServices.EmailExistsAsync(email);
            return Ok(new { exists });
        }

        [HttpGet("check-phone")]
        public async Task<IActionResult> CheckPhone([FromQuery] string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(new { exists = false });

            var exists = await _userServices.PhoneExistsAsync(phone);
            return Ok(new { exists });
        }
    }

}
