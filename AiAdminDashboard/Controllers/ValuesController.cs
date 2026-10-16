using Microsoft.AspNetCore.Mvc;

namespace AiAdminDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        [HttpGet("status")]
        public IActionResult GetSystemStatus()
        {
            var mockStatus = new
            {
                SystemName = "YVES AI",
                Status = "Online",
                UptimeSeconds = 14205,
                MemoryUsageMb = 412.5,
                ActiveModule = "Idle Listening",
                LastMemoryLogged = "User discussed C# Web API setup"
            };

            return Ok(mockStatus);
        }
    }
}
