using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using System.Security.Claims;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceTaskController : ControllerBase
{
    private readonly AppDbContext _context;

    public MaintenanceTaskController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/MaintenanceTask
    // Manager sees all tasks. Technician sees only tasks assigned to them (#51).
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaintenanceTaskResponse>>> GetTasks()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var isManager = User.IsInRole("Manager");

        var query = _context.MaintenanceTasks.AsQueryable();

        if (!isManager)
        {
            query = query.Where(t => t.AssignedToUserId == userId);
        }

        var tasks = await query
            .Select(t => new MaintenanceTaskResponse
            {
                MaintenanceTaskId = t.MaintenanceTaskId,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                IncidentId = t.IncidentId,
                AssignedToUserId = t.AssignedToUserId
            })
            .ToListAsync();

        return tasks;
    }

    // GET: /api/MaintenanceTask/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<MaintenanceTaskResponse>> GetTask(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var isManager = User.IsInRole("Manager");

        var task = await _context.MaintenanceTasks
            .Where(t => t.MaintenanceTaskId == id)
            .FirstOrDefaultAsync();

        if (task == null)
        {
            return NotFound();
        }

        // Technician can only view their own assigned tasks (#51).
        if (!isManager && task.AssignedToUserId != userId)
        {
            return Forbid();
        }

        var response = new MaintenanceTaskResponse
        {
            MaintenanceTaskId = task.MaintenanceTaskId,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            IncidentId = task.IncidentId,
            AssignedToUserId = task.AssignedToUserId
        };

        return response;
    }

    // POST: /api/MaintenanceTask
    // Manager creates and assigns a task to a technician (#50).
    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<ActionResult<MaintenanceTaskResponse>> CreateTask(
        CreateMaintenanceTaskRequest request)
    {
        var incidentExists = await _context.Incidents
            .AnyAsync(i => i.IncidentId == request.IncidentId);

        if (!incidentExists)
        {
            return NotFound(
                $"Incident with id {request.IncidentId} was not found.");
        }

        var assigneeExists = await _context.Users
            .AnyAsync(u => u.UserId == request.AssignedToUserId);

        if (!assigneeExists)
        {
            return NotFound(
                $"User with id {request.AssignedToUserId} was not found.");
        }

        var task = new MaintenanceTask
        {
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            IncidentId = request.IncidentId,
            AssignedToUserId = request.AssignedToUserId
        };

        _context.MaintenanceTasks.Add(task);
        await _context.SaveChangesAsync();

        var response = new MaintenanceTaskResponse
        {
            MaintenanceTaskId = task.MaintenanceTaskId,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            IncidentId = task.IncidentId,
            AssignedToUserId = task.AssignedToUserId
        };

        return CreatedAtAction(
            nameof(GetTask),
            new { id = task.MaintenanceTaskId },
            response);
    }

    // PUT: /api/MaintenanceTask/{id}
    // Status & completion tracking (#52). Technician can update their own
    // assigned task's status; Manager can update any task.
    [HttpPut("{id}")]
    public async Task<ActionResult<MaintenanceTaskResponse>> UpdateTask(
        int id,
        UpdateMaintenanceTaskRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var isManager = User.IsInRole("Manager");

        var task = await _context.MaintenanceTasks
            .FirstOrDefaultAsync(t => t.MaintenanceTaskId == id);

        if (task == null)
        {
            return NotFound();
        }

        if (!isManager && task.AssignedToUserId != userId)
        {
            return Forbid();
        }

        task.Status = request.Status;
        task.CompletedAt = request.Status == "Completed"
            ? (request.CompletedAt ?? DateTime.UtcNow)
            : request.CompletedAt;

        await _context.SaveChangesAsync();

        var response = new MaintenanceTaskResponse
        {
            MaintenanceTaskId = task.MaintenanceTaskId,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            IncidentId = task.IncidentId,
            AssignedToUserId = task.AssignedToUserId
        };

        return Ok(response);
    }
}
