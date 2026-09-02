using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PriorityScoreService _priorityScoreService;

    public MachineController(
        AppDbContext context,
        PriorityScoreService priorityScoreService)
    {
        _context = context;
        _priorityScoreService = priorityScoreService;
    }

    // GET: /api/Machine
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MachineResponse>>> GetMachines()
    {
        var machines = await _context.Machines
            .Include(m => m.Location)
            .Include(m => m.Incidents)
            .ToListAsync();

        var responses = machines.Select(m =>
        {
            var priority = _priorityScoreService.Calculate(m.Incidents);

            return new MachineResponse
            {
                MachineId = m.MachineId,
                Name = m.Name,
                Status = m.Status,
                Runtime = m.Runtime,
                LocationId = m.LocationId,
                Location = m.Location == null
                    ? null
                    : new LocationResponse
                    {
                        LocationId = m.Location.LocationId,
                        Name = m.Location.Name,
                        Address = m.Location.Address
                    },
                PriorityScore = priority.Score,
                PriorityBucket = priority.Bucket
            };
        }).ToList();

        return responses;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<MachineResponse>> GetMachine(int id)
    {
        var machine = await _context.Machines
            .Include(m => m.Location)
            .Include(m => m.Incidents)
            .FirstOrDefaultAsync(m => m.MachineId == id);

        if (machine == null)
        {
            return NotFound();
        }

        return machine;
    }

    // POST: /api/Machine
    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<ActionResult<Machine>> CreateMachine(
        CreateMachineRequest request)
    {
        var locationExists = await _context.Locations
            .AnyAsync(l => l.LocationId == request.LocationId);

        if (!locationExists)
        {
            return NotFound("Location not found.");
        }

        var machine = new Machine
        {
            Name = request.Name,
            Status = request.Status,
            Runtime = request.Runtime,
            LocationId = request.LocationId
        };

        _context.Machines.Add(machine);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMachine),
            new { id = machine.MachineId },
            machine);
    }

    // PUT: /api/Machine/{id}
    [HttpPut("{id}")]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> UpdateMachine(
        int id,
        UpdateMachineRequest request)
    {
        var machine = await _context.Machines
            .FirstOrDefaultAsync(m => m.MachineId == id);

        if (machine == null)
        {
            return NotFound();
        }

        var locationExists = await _context.Locations
            .AnyAsync(l => l.LocationId == request.LocationId);

        if (!locationExists)
        {
            return NotFound("Location not found.");
        }

        machine.Name = request.Name;
        machine.Status = request.Status;
        machine.Runtime = request.Runtime;
        machine.LocationId = request.LocationId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: /api/Machine/{id}
    [HttpDelete("{id}")]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> DeleteMachine(int id)
    {
        var machine = await _context.Machines
            .FirstOrDefaultAsync(m => m.MachineId == id);

        if (machine == null)
        {
            return NotFound();
        }

        _context.Machines.Remove(machine);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
