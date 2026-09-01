using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/Dashboard/stats
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsResponse>> GetStats()
    {
        var totalMachines = await _context.Machines.CountAsync();

        var openIncidents = await _context.Incidents
            .CountAsync(i => i.Status == "Open");

        return Ok(new DashboardStatsResponse
        {
            TotalMachines = totalMachines,
            OpenIncidents = openIncidents
        });
    }
    // GET: /api/Dashboard/pipeline
    [HttpGet("pipeline")]
    public async Task<ActionResult<LatestPipelineResponse>> GetLatestPipeline()
    {
        var latestPipeline = await _context.PipelineRuns
            .OrderByDescending(p => p.StartedAt)
            .FirstOrDefaultAsync();

        if (latestPipeline == null)
        {
            return NotFound();
        }

        return Ok(new LatestPipelineResponse
        {
            Status = latestPipeline.Status,
            DataQualityPct = latestPipeline.DataQualityPct
        });
    }
}
