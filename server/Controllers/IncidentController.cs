using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using System.Security.Claims;
using server.Services;
namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IIncidentAiService _aiService;
    private readonly IncidentAiSuggestionValidator _aiValidator;

    public IncidentController(
        AppDbContext context,
        IIncidentAiService aiService,
        IncidentAiSuggestionValidator aiValidator)
    {
        _context = context;
        _aiService = aiService;
        _aiValidator = aiValidator;
    }

    // GET: /api/Incident
    [HttpGet]
    [Authorize(Policy = "IncidentAccess")]
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
                MachineName = i.Machine == null ? string.Empty : i.Machine.Name,
                ReportedByUserId = i.ReportedByUserId
            })
            .ToListAsync();

        return incidents;
    }

    // GET: /api/Incident/{id}
    [HttpGet("{id}")]
    [Authorize(Policy = "IncidentAccess")]
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
                MachineName = i.Machine == null ? string.Empty : i.Machine.Name,
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
    [Authorize(Policy = "TechnicianOnly")]
    public async Task<ActionResult<IncidentResponse>> CreateIncident(
        CreateIncidentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }
        // Kontrollera att maskinen finns.
        var machine = await _context.Machines
            .FirstOrDefaultAsync(m => m.MachineId == request.MachineId);

        if (machine == null)
        {
            return NotFound(
                $"Machine with id {request.MachineId} was not found.");
        }

        // Kontrollera att användaren finns.
        var userExists = await _context.Users
            .AnyAsync(u => u.UserId == userId);

        if (!userExists)
        {
            return NotFound(
                $"User with id {userId} was not found.");
        }

        var aiCategory = request.Category;
        var aiPriority = request.Priority;
        var aiSuggestion = request.AiSuggestion;

        try
        {
            var machineContext =
                $"Name: {machine.Name}, Status: {machine.Status}, Runtime: {machine.Runtime}";

            var suggestion = await _aiService.SuggestAsync(
                request.Description,
                machineContext);

            if (suggestion != null && _aiValidator.IsValid(suggestion))
            {
                aiCategory = suggestion.Category;
                aiPriority = suggestion.Priority;
                aiSuggestion = suggestion.RecommendedAction;
            }
        }
        catch
        {
            // AI failure should not prevent incident creation.
        }

        // Skapa Incident-entiteten.
        var incident = new Incident
        {
            Description = request.Description,
            Status = request.Status,
            Category = aiCategory,
            Priority = aiPriority,
            AiSuggestion = aiSuggestion,
            CreatedAt = DateTime.UtcNow,
            ResolvedAt = request.ResolvedAt,
            MachineId = request.MachineId,
            ReportedByUserId = userId
        };

        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync();

        // Skapa response-objektet.
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
            MachineName = machine.Name,
            ReportedByUserId = incident.ReportedByUserId
        };

        return CreatedAtAction(
            nameof(GetIncident),
            new { id = incident.IncidentId },
            response);
    }

        // PUT: /api/Incident/{id}
    [HttpPut("{id}")]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<ActionResult<IncidentResponse>> UpdateIncident(
        int id,
        UpdateIncidentRequest request)
    {
        var incident = await _context.Incidents
            .Include(i => i.Machine)
            .FirstOrDefaultAsync(i => i.IncidentId == id);

        if (incident == null)
        {
            return NotFound();
        }

        incident.Status = request.Status;
        incident.Priority = request.Priority;
        incident.Category = request.Category;
        incident.AiSuggestion = request.AiSuggestion;
        incident.ResolvedAt = request.ResolvedAt;

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
            MachineName = incident.Machine?.Name ?? string.Empty,
            ReportedByUserId = incident.ReportedByUserId
        };

        return Ok(response);
    }
}