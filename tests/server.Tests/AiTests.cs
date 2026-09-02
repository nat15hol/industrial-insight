using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Controllers;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services;

namespace server.Tests;

public class AiTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"server-tests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<(User User, Machine Machine)> SeedIncidentData(
        AppDbContext context)
    {
        var role = new Role
        {
            Name = "Technician"
        };

        var location = new Location
        {
            Name = "Test Location",
            Address = "Test Address"
        };

        var user = new User
        {
            Name = "Test Technician",
            Email = "technician@test.local",
            PasswordHash = "test-hash",
            Role = role
        };

        var machine = new Machine
        {
            Name = "Test Machine",
            Status = "Operational",
            Runtime = 10,
            Location = location
        };

        context.Roles.Add(role);
        context.Users.Add(user);
        context.Locations.Add(location);
        context.Machines.Add(machine);

        await context.SaveChangesAsync();

        return (user, machine);
    }

    private static ClaimsPrincipal CreateTechnicianPrincipal(int userId)
    {
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Technician")
            },
            authenticationType: "Test");

        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task Valid_ai_suggestion_is_applied_to_incident()
    {
        await using var context = CreateContext();

        var (user, machine) = await SeedIncidentData(context);

        var controller = new IncidentController(
            context,
            new MockIncidentAiService(),
            new IncidentAiSuggestionValidator());

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateTechnicianPrincipal(user.UserId)
            }
        };

        var request = new CreateIncidentRequest
        {
            Description = "oil leak",
            Status = "Open",
            Priority = "Low",
            Category = "Other",
            AiSuggestion = null,
            ResolvedAt = null,
            MachineId = machine.MachineId
        };

        var result = await controller.CreateIncident(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<IncidentResponse>(createdResult.Value);

        Assert.Equal("Mechanical", response.Category);
        Assert.Equal("High", response.Priority);
        Assert.Equal(
            "Inspect the machine for oil leakage and mechanical damage.",
            response.AiSuggestion);

        var incident = await context.Incidents
            .SingleAsync(i => i.IncidentId == response.IncidentId);

        Assert.Equal("Mechanical", incident.Category);
        Assert.Equal("High", incident.Priority);
        Assert.Equal(
            "Inspect the machine for oil leakage and mechanical damage.",
            incident.AiSuggestion);
    }

    [Fact]
    public async Task Ai_failure_does_not_prevent_incident_creation()
    {
        await using var context = CreateContext();

        var (user, machine) = await SeedIncidentData(context);

        var controller = new IncidentController(
            context,
            new FailingIncidentAiService(),
            new IncidentAiSuggestionValidator());

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateTechnicianPrincipal(user.UserId)
            }
        };

        var request = new CreateIncidentRequest
        {
            Description = "AI failure test",
            Status = "Open",
            Priority = "Low",
            Category = "Other",
            AiSuggestion = "Manual fallback suggestion",
            ResolvedAt = null,
            MachineId = machine.MachineId
        };

        var result = await controller.CreateIncident(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<IncidentResponse>(createdResult.Value);

        Assert.Equal("Low", response.Priority);
        Assert.Equal("Other", response.Category);
        Assert.Equal("Manual fallback suggestion", response.AiSuggestion);

        var incident = await context.Incidents
            .SingleAsync(i => i.IncidentId == response.IncidentId);

        Assert.Equal("Low", incident.Priority);
        Assert.Equal("Other", incident.Category);
        Assert.Equal("Manual fallback suggestion", incident.AiSuggestion);
    }

    private sealed class FailingIncidentAiService : IIncidentAiService
    {
        public Task<IncidentAiSuggestionDto?> SuggestAsync(
            string description,
            string machineContext)
        {
            throw new InvalidOperationException("Simulated AI failure");
        }
    }

    [Fact]
    public async Task Invalid_ai_suggestion_does_not_override_incident_values()
    {
        await using var context = CreateContext();

        var (user, machine) = await SeedIncidentData(context);

        var controller = new IncidentController(
            context,
            new InvalidIncidentAiService(),
            new IncidentAiSuggestionValidator());

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateTechnicianPrincipal(user.UserId)
            }
        };

        var request = new CreateIncidentRequest
        {
            Description = "Malformed AI response test",
            Status = "Open",
            Priority = "Low",
            Category = "Other",
            AiSuggestion = "Manual fallback suggestion",
            ResolvedAt = null,
            MachineId = machine.MachineId
        };

        var result = await controller.CreateIncident(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<IncidentResponse>(createdResult.Value);

        Assert.Equal("Low", response.Priority);
        Assert.Equal("Other", response.Category);
        Assert.Equal("Manual fallback suggestion", response.AiSuggestion);

        var incident = await context.Incidents
            .SingleAsync(i => i.IncidentId == response.IncidentId);

        Assert.Equal("Low", incident.Priority);
        Assert.Equal("Other", incident.Category);
        Assert.Equal("Manual fallback suggestion", incident.AiSuggestion);
    }

    private sealed class InvalidIncidentAiService : IIncidentAiService
    {
        public Task<IncidentAiSuggestionDto?> SuggestAsync(
            string description,
            string machineContext)
        {
            return Task.FromResult<IncidentAiSuggestionDto?>(
                new IncidentAiSuggestionDto
                {
                    Category = "InvalidCategory",
                    Priority = "Critical",
                    RecommendedAction = ""
                });
        }
    }
}