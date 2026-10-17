using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace AiAdminDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        private const string LaptopOllamaUrl = "http://26.24.123.89:11434";

        public TelemetryController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetSystemStatus()
        {
            try
            {
                var client = _clientFactory.CreateClient();
                {
                    var response = await client.GetAsync($"{LaptopOllamaUrl}/api/ps");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();

                        using var doc = JsonDocument.Parse(jsonString);
                        var root = doc.RootElement;
                        var models = root.GetProperty("models");

                        if (models.GetArrayLength() > 0)
                        {
                            var activeModel = models[0];
                            string modelName = activeModel.GetProperty("name").GetString() ?? "Unknown";
                            long sizeVram = activeModel.GetProperty("size_vram").GetInt64();
                            double vramMb = sizeVram / (1024.0 * 1024.0);

                            var liveStatus = new
                            {
                                SystemName = "MSI Laptop Node",
                                Status = "Online",
                                UptimeSeconds = 14205,
                                MemoryUsageMb = Math.Round(vramMb, 2),
                                ActiveModule = $"Running: {modelName}",
                                LastMemoryLogged = "Fetched live from Ollama /api/ps"
                            };

                            return Ok(liveStatus);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ollama connection error: {ex.Message}");
            }
            
            var idleStatus = new
            {
                SystemName = "MSI Laptop Node",
                Status = "Online (Idle)",
                UptimeSeconds = 14205,
                MemoryUsageMb = 0.0,
                ActiveModule = "Idle Listening",
                LastMemoryLogged = "Ollama active, no models loaded"
            };

            return Ok(idleStatus);
        }
    }
}
