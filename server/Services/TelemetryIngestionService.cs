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

            var seenRecords = new HashSet<(int MachineId, DateTime Timestamp)>();

            while (await csv.ReadAsync())
            {
                pipelineRun.RecordsProcessed++;

                TelemetryRecord? record = null;

                try
                {
                    record = csv.GetRecord<TelemetryRecord>();
                }
                catch
                {
                    pipelineRun.RecordsRejected++;
                    continue;
                }

                if (!IsDomainValid(record))
                {
                    pipelineRun.RecordsRejected++;
                    continue;
                }

                var machineExists = await _context.Machines
                    .AnyAsync(m => m.MachineId == record.MachineId);

                if (!machineExists)
                {
                    pipelineRun.RecordsRejected++;
                    continue;
                }

                var key = (record.MachineId, record.Timestamp);

                if (!seenRecords.Add(key))
                {
                    pipelineRun.Duplicates++;
                    continue;
                }

                _context.TelemetryRecords.Add(record);
                pipelineRun.RecordsAccepted++;
            }

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

    private static bool IsDomainValid(TelemetryRecord record)
    {
        return record.Temperature >= 0 && record.Temperature <= 120
            && record.Pressure >= 0 && record.Pressure <= 10
            && record.Vibration >= 0 && record.Vibration <= 5
            && record.Energy >= 0 && record.Energy <= 30;
    }
}