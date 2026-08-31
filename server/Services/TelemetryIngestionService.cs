using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using System.Globalization;

namespace server.Services;

public class TelemetryIngestionService : ITelemetryIngestionService
{
    private readonly AppDbContext _context;

    public TelemetryIngestionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TelemetryIngestionResult> IngestAsync(Stream csvStream)
    {
        var pipelineRun = new PipelineRun
        {
            StartedAt = DateTime.UtcNow,
            Status = "Running"
        };

        _context.PipelineRuns.Add(pipelineRun);
        await _context.SaveChangesAsync();

        try
        {
            using var reader = new StreamReader(csvStream);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<TelemetryRecordMap>();

            // Validate required CSV headers before processing any records.
            if (!await csv.ReadAsync())
            {
                throw new InvalidDataException("The CSV file is empty.");
            }

            csv.ReadHeader();

            var requiredHeaders = new[]
            {
                "MachineId",
                "Timestamp",
                "Temperature",
                "Pressure",
                "Vibration",
                "Energy"
            };

            foreach (var header in requiredHeaders)
            {
                if (csv.HeaderRecord is null ||
                    !csv.HeaderRecord.Contains(header, StringComparer.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException(
                        $"Required CSV column '{header}' is missing.");
                }
            }

            var processedKeys = new HashSet<(int MachineId, DateTime Timestamp)>();

            while (await csv.ReadAsync())
            {
                pipelineRun.RecordsProcessed++;

                var requiredFields = new[]
                {
                    "MachineId",
                    "Timestamp",
                    "Temperature",
                    "Pressure",
                    "Vibration",
                    "Energy"
                };

                var hasMissingField = requiredFields.Any(field =>
                {
                    var index = csv.GetFieldIndex(field, isTryGet: true);
                    return index < 0 || string.IsNullOrWhiteSpace(csv.GetField(index) ?? string.Empty);
                });

                if (hasMissingField)
                {
                    Console.WriteLine($"CSV REJECTED: Missing required value on row {csv.Context.Parser?.Row}");
                    pipelineRun.RecordsRejected++;
                    continue;
                }

                TelemetryRecord? record = null;

                try
                {
                    record = csv.GetRecord<TelemetryRecord>();

                    if (record is null)
                    {
                        pipelineRun.RecordsRejected++;
                        continue;
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"CSV REJECTED: {ex.Message}");
                    pipelineRun.RecordsRejected++;
                    continue;
                }

                var machineExists = await _context.Machines
                    .AnyAsync(m => m.MachineId == record.MachineId);

                if (!machineExists)
                {
                    Console.WriteLine(
                        $"REJECTED MACHINE: Machine={record.MachineId}, " +
                        $"Timestamp={record.Timestamp:o}");

                    pipelineRun.RecordsRejected++;
                    continue;
                }

                if (!HasValidMeasurements(record))
                {
                    Console.WriteLine(
                        $"REJECTED RANGE: Machine={record.MachineId}, " +
                        $"Timestamp={record.Timestamp:o}, " +
                        $"Temp={record.Temperature}, Pressure={record.Pressure}, " +
                        $"Vibration={record.Vibration}, Energy={record.Energy}");

                    pipelineRun.RecordsRejected++;
                    continue;
                }

                var recordKey = (record.MachineId, record.Timestamp);

                if (!processedKeys.Add(recordKey))
                {
                    pipelineRun.Duplicates++;
                    continue;
                }

                var duplicateExists = await _context.TelemetryRecords
                    .AnyAsync(t =>
                        t.MachineId == record.MachineId &&
                        t.Timestamp == record.Timestamp);

                if (duplicateExists)
                {
                    pipelineRun.Duplicates++;
                    continue;
                }

                _context.TelemetryRecords.Add(record);
                pipelineRun.RecordsAccepted++;
            }

            pipelineRun.DataQualityPct = pipelineRun.RecordsProcessed == 0
                ? 0
                : (float)pipelineRun.RecordsAccepted
                    / pipelineRun.RecordsProcessed * 100;

            pipelineRun.FinishedAt = DateTime.UtcNow;
            pipelineRun.Status = "Completed";

            await _context.SaveChangesAsync();

            return new TelemetryIngestionResult
            {
                PipelineRunId = pipelineRun.PipelineRunId,
                RecordsProcessed = pipelineRun.RecordsProcessed,
                RecordsAccepted = pipelineRun.RecordsAccepted,
                RecordsRejected = pipelineRun.RecordsRejected,
                Duplicates = pipelineRun.Duplicates,
                DataQualityPct = pipelineRun.DataQualityPct,
                Status = pipelineRun.Status
            };
        }
        catch
        {
            pipelineRun.FinishedAt = DateTime.UtcNow;
            pipelineRun.Status = "Failed";

            await _context.SaveChangesAsync();

            throw;
        }
    }
    private static bool HasValidMeasurements(TelemetryRecord record)
    {
        return record.Timestamp >= DateTime.UtcNow.AddYears(-10)
            && record.Timestamp <= DateTime.UtcNow.AddDays(1)
            && record.Temperature >= -50
            && record.Temperature <= 150
            && record.Pressure >= 0
            && record.Pressure <= 20
            && record.Vibration >= 0
            && record.Vibration <= 10
            && record.Energy >= 0
            && record.Energy <= 100;
    }
}