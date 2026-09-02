using server.DTOs;
using server.Models;

namespace server.Services;

public class AnomalyDetectionService : IAnomalyDetectionService
{
    public IReadOnlyList<AnomalyDetectionResponse> Detect(
        IEnumerable<TelemetryRecord> records)
    {
        var telemetry = records.ToList();

        if (telemetry.Count == 0)
        {
            return Array.Empty<AnomalyDetectionResponse>();
        }

        var temperatureStats = CalculateStats(
            telemetry.Select(t => t.Temperature));

        var pressureStats = CalculateStats(
            telemetry.Select(t => t.Pressure));

        var vibrationStats = CalculateStats(
            telemetry.Select(t => t.Vibration));

        var energyStats = CalculateStats(
            telemetry.Select(t => t.Energy));

        return telemetry
            .Select(record => new AnomalyDetectionResponse
            {
                TelemetryRecordId = record.TelemetryRecordId,
                MachineId = record.MachineId,
                Timestamp = record.Timestamp,

                Temperature = record.Temperature,
                Pressure = record.Pressure,
                Vibration = record.Vibration,
                Energy = record.Energy,

                TemperatureAnomaly = IsOutsideTwoStandardDeviations(
                    record.Temperature,
                    temperatureStats),

                PressureAnomaly = IsOutsideTwoStandardDeviations(
                    record.Pressure,
                    pressureStats),

                VibrationAnomaly = IsOutsideTwoStandardDeviations(
                    record.Vibration,
                    vibrationStats),

                EnergyAnomaly = IsOutsideTwoStandardDeviations(
                    record.Energy,
                    energyStats)
            })
            .ToList();
    }

    private static (double Mean, double StandardDeviation) CalculateStats(
        IEnumerable<float> values)
    {
        var data = values.Select(value => (double)value).ToList();

        if (data.Count == 0)
        {
            return (0, 0);
        }

        var mean = data.Average();

        if (data.Count == 1)
        {
            return (mean, 0);
        }

        var variance = data
            .Sum(value => Math.Pow(value - mean, 2))
            / (data.Count - 1);

        return (mean, Math.Sqrt(variance));
    }

    private static bool IsOutsideTwoStandardDeviations(
        float value,
        (double Mean, double StandardDeviation) stats)
    {
        if (stats.StandardDeviation == 0)
        {
            return false;
        }

        var lowerBound = stats.Mean - (2 * stats.StandardDeviation);
        var upperBound = stats.Mean + (2 * stats.StandardDeviation);

        return value < lowerBound || value > upperBound;
    }
}