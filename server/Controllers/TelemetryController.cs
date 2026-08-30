using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Services;

namespace server.Controllers;

[ApiController]
[Route("api/telemetry")]
[Authorize(Roles = "Manager")]
public class TelemetryController : ControllerBase
{
    private readonly ITelemetryIngestionService _telemetryIngestionService;

    public TelemetryController(
        ITelemetryIngestionService telemetryIngestionService)
    {
        _telemetryIngestionService = telemetryIngestionService;
    }

    [HttpPost("ingest")]
    public async Task<IActionResult> Ingest(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                error = "invalid_file",
                message = "A CSV file is required."
            });
        }

        var result = await _telemetryIngestionService.IngestAsync(file.OpenReadStream());

        return Ok(result);
    }
}