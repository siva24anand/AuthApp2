using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        [HttpGet("secure")]
        public IActionResult GetSecureData()
        {
            var username = User.Identity?.Name ?? "Unknown";
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

            return Ok(new
            {
                Message = "This is secure data.",
                User = username,
                IsAuthenticated = isAuthenticated
            });
        }
    }
}
