using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentController : ControllerBase
{
    private readonly AppDbContext _context;

    public IncidentController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/Incident
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncidentResponse>>> GetIncidents()
    {
        var incidents = await _context.Incidents
            .Select(i => new IncidentResponse
            {
                IncidentId = i.IncidentId,
                Description = i.Description,
                Status = i.Status,
                Priority = i.Priority,
                Category = i.Category,
                AiSuggestion = i.AiSuggestion,
                CreatedAt = i.CreatedAt,
                ResolvedAt = i.ResolvedAt,
                MachineId = i.MachineId,
                ReportedByUserId = i.ReportedByUserId
            })
            .ToListAsync();

        return incidents;
    }

    // GET: /api/Incident/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<IncidentResponse>> GetIncident(int id)
    {
        var incident = await _context.Incidents
            .Where(i => i.IncidentId == id)
            .Select(i => new IncidentResponse
            {
                IncidentId = i.IncidentId,
                Description = i.Description,
                Status = i.Status,
                Priority = i.Priority,
                Category = i.Category,
                AiSuggestion = i.AiSuggestion,
                CreatedAt = i.CreatedAt,
                ResolvedAt = i.ResolvedAt,
                MachineId = i.MachineId,
                ReportedByUserId = i.ReportedByUserId
            })
            .FirstOrDefaultAsync();

        if (incident == null)
        {
            return NotFound();
        }

        return incident;
    }

    // POST: /api/Incident
    [HttpPost]
    public async Task<ActionResult<IncidentResponse>> CreateIncident(
        CreateIncidentRequest request)
    {
        var incident = new Incident
        {
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            Category = request.Category,
            AiSuggestion = request.AiSuggestion,
            CreatedAt = DateTime.UtcNow,
            ResolvedAt = request.ResolvedAt,
            MachineId = request.MachineId,
            ReportedByUserId = request.ReportedByUserId
        };

        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync();

        var response = new IncidentResponse
        {
            IncidentId = incident.IncidentId,
            Description = incident.Description,
            Status = incident.Status,
            Priority = incident.Priority,
            Category = incident.Category,
            AiSuggestion = incident.AiSuggestion,
            CreatedAt = incident.CreatedAt,
            ResolvedAt = incident.ResolvedAt,
            MachineId = incident.MachineId,
            ReportedByUserId = incident.ReportedByUserId
        };

        return CreatedAtAction(
            nameof(GetIncident),
            new { id = incident.IncidentId },
            response);
    }
}