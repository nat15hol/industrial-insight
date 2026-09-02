using System.Text;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using server.Services;

namespace server.Tests;

public class DataQualityTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"server-tests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<Machine> SeedMachine(AppDbContext context)
    {
        var location = new Location
        {
            Name = "Test Location",
            Address = "Test Address"
        };

        var machine = new Machine
        {
            Name = "Test Machine",
            Status = "Operational",
            Runtime = 10,
            Location = location
        };

        context.Locations.Add(location);
        context.Machines.Add(machine);

        await context.SaveChangesAsync();

        return machine;
    }

    [Fact]
    public async Task Duplicate_rows_within_same_csv_are_counted_once()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var timestamp = "2026-09-02T12:00:00Z";

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},{timestamp},75,5,2,40
            {machine.MachineId},{timestamp},75,5,2,40
            """;

        await using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        var service = new TelemetryIngestionService(context);

        var result = await service.IngestAsync(stream);

        Assert.Equal("Completed", result.Status);
        Assert.Equal(2, result.RecordsProcessed);
        Assert.Equal(1, result.RecordsAccepted);
        Assert.Equal(0, result.RecordsRejected);
        Assert.Equal(1, result.Duplicates);
        Assert.Equal(50f, result.DataQualityPct);

        var telemetryRecords = await context.TelemetryRecords
            .ToListAsync();

        Assert.Single(telemetryRecords);

        Assert.Equal(machine.MachineId, telemetryRecords[0].MachineId);
        Assert.Equal(75f, telemetryRecords[0].Temperature);
        Assert.Equal(5f, telemetryRecords[0].Pressure);
        Assert.Equal(2f, telemetryRecords[0].Vibration);
        Assert.Equal(40f, telemetryRecords[0].Energy);

        var pipeline = await context.PipelineRuns
            .SingleAsync(p => p.PipelineRunId == result.PipelineRunId);

        Assert.Equal("Completed", pipeline.Status);
        Assert.Equal(2, pipeline.RecordsProcessed);
        Assert.Equal(1, pipeline.RecordsAccepted);
        Assert.Equal(0, pipeline.RecordsRejected);
        Assert.Equal(1, pipeline.Duplicates);
        Assert.Equal(50f, pipeline.DataQualityPct);
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Duplicate_existing_in_database_is_not_persisted_again()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var timestamp = new DateTime(2026, 9, 2, 14, 0, 0, DateTimeKind.Local);

        var existingTelemetry = new TelemetryRecord
        {
            MachineId = machine.MachineId,
            Timestamp = timestamp,
            Temperature = 75,
            Pressure = 5,
            Vibration = 2,
            Energy = 40
        };

        context.TelemetryRecords.Add(existingTelemetry);
        await context.SaveChangesAsync();

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},{timestamp:O},75,5,2,40
            """;

        await using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        var service = new TelemetryIngestionService(context);

        var result = await service.IngestAsync(stream);

        Assert.Equal("Completed", result.Status);
        Assert.Equal(1, result.RecordsProcessed);
        Assert.Equal(0, result.RecordsAccepted);
        Assert.Equal(0, result.RecordsRejected);
        Assert.Equal(1, result.Duplicates);
        Assert.Equal(0f, result.DataQualityPct);

        var telemetryRecords = await context.TelemetryRecords
            .ToListAsync();

        Assert.Single(telemetryRecords);

        Assert.Equal(machine.MachineId, telemetryRecords[0].MachineId);
        Assert.Equal(timestamp, telemetryRecords[0].Timestamp);
        Assert.Equal(75f, telemetryRecords[0].Temperature);
        Assert.Equal(5f, telemetryRecords[0].Pressure);
        Assert.Equal(2f, telemetryRecords[0].Vibration);
        Assert.Equal(40f, telemetryRecords[0].Energy);

        var pipeline = await context.PipelineRuns
            .SingleAsync(p => p.PipelineRunId == result.PipelineRunId);

        Assert.Equal("Completed", pipeline.Status);
        Assert.Equal(1, pipeline.RecordsProcessed);
        Assert.Equal(0, pipeline.RecordsAccepted);
        Assert.Equal(0, pipeline.RecordsRejected);
        Assert.Equal(1, pipeline.Duplicates);
        Assert.Equal(0f, pipeline.DataQualityPct);
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Pressure_below_minimum_is_rejected()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},2026-09-02T12:00:00Z,75,-1,2,40
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
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Pressure_above_maximum_is_rejected()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},2026-09-02T12:00:00Z,75,21,2,40
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
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Vibration_below_minimum_is_rejected()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},2026-09-02T12:00:00Z,75,5,-1,40
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
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Vibration_above_maximum_is_rejected()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},2026-09-02T12:00:00Z,75,5,11,40
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
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Energy_below_minimum_is_rejected()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},2026-09-02T12:00:00Z,75,5,2,-1
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
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Energy_above_maximum_is_rejected()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},2026-09-02T12:00:00Z,75,5,2,101
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
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Timestamp_older_than_ten_years_is_rejected()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var oldTimestamp = DateTime.UtcNow.AddYears(-11)
            .ToString("O");

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},{oldTimestamp},75,5,2,40
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
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Timestamp_more_than_one_day_in_future_is_rejected()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var futureTimestamp = DateTime.UtcNow.AddDays(2)
            .ToString("O");

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},{futureTimestamp},75,5,2,40
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
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Pressure_boundary_values_are_accepted()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {machine.MachineId},2026-09-02T12:00:00Z,75,0,2,40
            {machine.MachineId},2026-09-02T13:00:00Z,75,20,2,40
            """;

        await using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(csv));

        var service = new TelemetryIngestionService(context);

        var result = await service.IngestAsync(stream);

        Assert.Equal("Completed", result.Status);
        Assert.Equal(2, result.RecordsProcessed);
        Assert.Equal(2, result.RecordsAccepted);
        Assert.Equal(0, result.RecordsRejected);
        Assert.Equal(0, result.Duplicates);
        Assert.Equal(100f, result.DataQualityPct);

        var telemetryRecords = await context.TelemetryRecords
            .OrderBy(t => t.Timestamp)
            .ToListAsync();

        Assert.Equal(2, telemetryRecords.Count);
        Assert.Equal(0f, telemetryRecords[0].Pressure);
        Assert.Equal(20f, telemetryRecords[1].Pressure);

        var pipeline = await context.PipelineRuns
            .SingleAsync(p => p.PipelineRunId == result.PipelineRunId);

        Assert.Equal("Completed", pipeline.Status);
        Assert.Equal(2, pipeline.RecordsProcessed);
        Assert.Equal(2, pipeline.RecordsAccepted);
        Assert.Equal(0, pipeline.RecordsRejected);
        Assert.Equal(0, pipeline.Duplicates);
        Assert.Equal(100f, pipeline.DataQualityPct);
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Telemetry_with_unknown_machine_is_rejected()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

        var unknownMachineId = machine.MachineId + 1000;

        var csv = $"""
            MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
            {unknownMachineId},2026-09-02T12:00:00Z,75,5,2,40
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
        Assert.NotNull(pipeline.FinishedAt);
    }

    [Fact]
    public async Task Accepted_telemetry_record_is_linked_to_correct_machine()
    {
        await using var context = CreateContext();

        var machine = await SeedMachine(context);

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
            .Include(t => t.Machine)
            .SingleAsync();

        Assert.Equal(machine.MachineId, telemetry.MachineId);
        Assert.NotNull(telemetry.Machine);
        Assert.Equal(machine.MachineId, telemetry.Machine!.MachineId);
        Assert.Equal(machine.Name, telemetry.Machine.Name);
        Assert.Equal(machine.Status, telemetry.Machine.Status);
        Assert.Equal(machine.LocationId, telemetry.Machine.LocationId);

        Assert.Single(machine.TelemetryRecords);
        Assert.Equal(telemetry.TelemetryRecordId, machine.TelemetryRecords.Single().TelemetryRecordId);
    }

    [Fact]
    public async Task Persisted_telemetry_record_loads_correct_machine_relationship()
    {
        var databaseName = $"server-tests-{Guid.NewGuid()}";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        int machineId;
        int telemetryRecordId;

        await using (var context = new AppDbContext(options))
        {
            var machine = await SeedMachine(context);
            machineId = machine.MachineId;

            var csv = $"""
                MachineId,Timestamp,Temperature,Pressure,Vibration,Energy
                {machine.MachineId},2026-09-02T12:00:00Z,75,5,2,40
                """;

            await using var stream = new MemoryStream(
                Encoding.UTF8.GetBytes(csv));

            var service = new TelemetryIngestionService(context);

            var result = await service.IngestAsync(stream);

            Assert.Equal("Completed", result.Status);
            Assert.Equal(1, result.RecordsAccepted);

            var telemetry = await context.TelemetryRecords
                .SingleAsync();

            telemetryRecordId = telemetry.TelemetryRecordId;
        }

        await using (var verificationContext = new AppDbContext(options))
        {
            var telemetry = await verificationContext.TelemetryRecords
                .Include(t => t.Machine)
                .SingleAsync(t => t.TelemetryRecordId == telemetryRecordId);

            Assert.Equal(machineId, telemetry.MachineId);
            Assert.NotNull(telemetry.Machine);
            Assert.Equal(machineId, telemetry.Machine!.MachineId);
            Assert.Equal("Test Machine", telemetry.Machine.Name);
            Assert.Equal("Operational", telemetry.Machine.Status);
        }
    }

}