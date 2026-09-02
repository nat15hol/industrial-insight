using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using server.Controllers;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services;
using Microsoft.AspNetCore.Http;

namespace server.Tests;

public class IncidentPipelineTests
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

    private sealed class NoOpIncidentAiService : IIncidentAiService
    {
        public Task<IncidentAiSuggestionDto?> SuggestAsync(
            string description,
            string machineContext)
        {
            return Task.FromResult<IncidentAiSuggestionDto?>(null);
        }
    }

    [Fact]
    public async Task Technician_can_create_incident_and_incident_is_persisted()
    {
        await using var context = CreateContext();

        var (user, machine) = await SeedIncidentData(context);

        var controller = new IncidentController(
            context,
            new NoOpIncidentAiService(),
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
            Description = "Test incident",
            Status = "Open",
            Priority = "Medium",
            Category = "Other",
            AiSuggestion = null,
            ResolvedAt = null,
            MachineId = machine.MachineId
        };

        var result = await controller.CreateIncident(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

        var response = Assert.IsType<IncidentResponse>(createdResult.Value);

        Assert.Equal(request.Description, response.Description);
        Assert.Equal(request.Status, response.Status);
        Assert.Equal(request.Priority, response.Priority);
        Assert.Equal(request.Category, response.Category);
        Assert.Equal(machine.MachineId, response.MachineId);
        Assert.Equal(user.UserId, response.ReportedByUserId);

        var incident = await context.Incidents
            .SingleOrDefaultAsync(i => i.IncidentId == response.IncidentId);

        Assert.NotNull(incident);
        Assert.Equal("Test incident", incident.Description);
        Assert.Equal("Open", incident.Status);
        Assert.Equal("Medium", incident.Priority);
        Assert.Equal("Other", incident.Category);
        Assert.Equal(machine.MachineId, incident.MachineId);
        Assert.Equal(user.UserId, incident.ReportedByUserId);
    }
    [Fact]
    public async Task Valid_csv_is_ingested_and_pipeline_completes()
    {
        await using var context = CreateContext();

        var (_, machine) = await SeedIncidentData(context);

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},2026-09-02T12:00:00Z,75,5,2,40
            """;

        await using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        var service = new TelemetryIngestionService(context);

        var result = await service.IngestAsync(stream);

        Assert.Equal("Completed", result.Status);
        Assert.Equal(1, result.RecordsProcessed);
        Assert.Equal(1, result.RecordsAccepted);
        Assert.Equal(0, result.RecordsRejected);
        Assert.Equal(0, result.Duplicates);
        Assert.Equal(100f, result.DataQualityPct);

        var telemetry = await context.TelemetryRecords
            .SingleOrDefaultAsync();

        Assert.NotNull(telemetry);
        Assert.Equal(machine.MachineId, telemetry.MachineId);
        Assert.Equal(75f, telemetry.Temperature);
        Assert.Equal(5f, telemetry.Pressure);
        Assert.Equal(2f, telemetry.Vibration);
        Assert.Equal(40f, telemetry.Energy);

        var pipeline = await context.PipelineRuns
            .SingleOrDefaultAsync(p => p.PipelineRunId == result.PipelineRunId);

        Assert.NotNull(pipeline);
        Assert.Equal("Completed", pipeline.Status);
        Assert.Equal(1, pipeline.RecordsProcessed);
        Assert.Equal(1, pipeline.RecordsAccepted);
        Assert.Equal(0, pipeline.RecordsRejected);
        Assert.Equal(0, pipeline.Duplicates);
        Assert.Equal(100f, pipeline.DataQualityPct);
    }

    [Fact]
    public async Task Csv_with_missing_required_header_fails_pipeline()
    {
        await using var context = CreateContext();

        var csv = """
            MachineId,Timestamp,Temperature,Pressure,Vibration
            1,2026-09-02T12:00:00Z,75,5,2
            """;

        await using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        var service = new TelemetryIngestionService(context);

        var exception = await Assert.ThrowsAsync<InvalidDataException>(
            () => service.IngestAsync(stream));

        Assert.Contains("Energy", exception.Message);

        var pipeline = await context.PipelineRuns
            .SingleAsync();

        Assert.Equal("Failed", pipeline.Status);
        Assert.Equal(0, pipeline.RecordsProcessed);
        Assert.Equal(0, pipeline.RecordsAccepted);
        Assert.Equal(0, pipeline.RecordsRejected);
        Assert.Equal(0, pipeline.Duplicates);
    }

    [Fact]
    public async Task Csv_with_invalid_measurement_is_rejected()
    {
        await using var context = CreateContext();

        var (_, machine) = await SeedIncidentData(context);

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},2026-09-02T12:00:00Z,999,5,2,40
            """;

        await using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        var service = new TelemetryIngestionService(context);

        var result = await service.IngestAsync(stream);

        Assert.Equal("Completed", result.Status);
        Assert.Equal(1, result.RecordsProcessed);
        Assert.Equal(0, result.RecordsAccepted);
        Assert.Equal(1, result.RecordsRejected);
        Assert.Equal(0, result.Duplicates);
        Assert.Equal(0f, result.DataQualityPct);

        Assert.Empty(context.TelemetryRecords);

        var pipeline = await context.PipelineRuns
            .SingleAsync(p => p.PipelineRunId == result.PipelineRunId);

        Assert.Equal("Completed", pipeline.Status);
        Assert.Equal(1, pipeline.RecordsProcessed);
        Assert.Equal(0, pipeline.RecordsAccepted);
        Assert.Equal(1, pipeline.RecordsRejected);
        Assert.Equal(0, pipeline.Duplicates);
        Assert.Equal(0f, pipeline.DataQualityPct);
    }
}