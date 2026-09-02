using server.Models;
using server.Services;

namespace server.Tests;

public class AnomalyDetectionTests
{
    [Fact]
    public void Empty_input_returns_no_results()
    {
        var service = new AnomalyDetectionService();

        var result = service.Detect(Array.Empty<TelemetryRecord>());

        Assert.Empty(result);
    }

    [Fact]
    public void Single_record_is_not_marked_as_anomaly()
    {
        var service = new AnomalyDetectionService();

        var records = new[]
        {
            CreateRecord(1, temperature: 50, pressure: 5, vibration: 2, energy: 50)
        };

        var result = service.Detect(records);

        var detection = Assert.Single(result);

        Assert.False(detection.TemperatureAnomaly);
        Assert.False(detection.PressureAnomaly);
        Assert.False(detection.VibrationAnomaly);
        Assert.False(detection.EnergyAnomaly);
        Assert.False(detection.IsAnomaly);
    }

    [Fact]
    public void Normal_values_are_not_marked_as_anomalies()
    {
        var service = new AnomalyDetectionService();

        var records = new[]
        {
            CreateRecord(1, 48, 4.8f, 1.8f, 48),
            CreateRecord(1, 50, 5.0f, 2.0f, 50),
            CreateRecord(1, 52, 5.2f, 2.2f, 52),
            CreateRecord(1, 49, 4.9f, 1.9f, 49),
            CreateRecord(1, 51, 5.1f, 2.1f, 51)
        };

        var result = service.Detect(records);

        Assert.All(result, detection =>
        {
            Assert.False(detection.TemperatureAnomaly);
            Assert.False(detection.PressureAnomaly);
            Assert.False(detection.VibrationAnomaly);
            Assert.False(detection.EnergyAnomaly);
            Assert.False(detection.IsAnomaly);
        });
    }

    [Fact]
    public void Temperature_value_outside_two_standard_deviations_is_anomaly()
    {
        var service = new AnomalyDetectionService();

        var records = new[]
        {
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 100, 5, 2, 50)
        };

        var result = service.Detect(records);

        var anomaly = result.Single(r => r.Temperature == 100);

        Assert.True(anomaly.TemperatureAnomaly);
        Assert.False(anomaly.PressureAnomaly);
        Assert.False(anomaly.VibrationAnomaly);
        Assert.False(anomaly.EnergyAnomaly);
        Assert.True(anomaly.IsAnomaly);
    }

    [Fact]
    public void Pressure_value_outside_two_standard_deviations_is_anomaly()
    {
        var service = new AnomalyDetectionService();

        var records = new[]
        {
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 20, 2, 50)
        };

        var result = service.Detect(records);

        var anomaly = result.Single(r => r.Pressure == 20);

        Assert.False(anomaly.TemperatureAnomaly);
        Assert.True(anomaly.PressureAnomaly);
        Assert.False(anomaly.VibrationAnomaly);
        Assert.False(anomaly.EnergyAnomaly);
        Assert.True(anomaly.IsAnomaly);
    }

    [Fact]
    public void Vibration_value_outside_two_standard_deviations_is_anomaly()
    {
        var service = new AnomalyDetectionService();

        var records = new[]
        {
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 10, 50)
        };

        var result = service.Detect(records);

        var anomaly = result.Single(r => r.Vibration == 10);

        Assert.False(anomaly.TemperatureAnomaly);
        Assert.False(anomaly.PressureAnomaly);
        Assert.True(anomaly.VibrationAnomaly);
        Assert.False(anomaly.EnergyAnomaly);
        Assert.True(anomaly.IsAnomaly);
    }

    [Fact]
    public void Energy_value_outside_two_standard_deviations_is_anomaly()
    {
        var service = new AnomalyDetectionService();

        var records = new[]
        {
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 50),
            CreateRecord(1, 50, 5, 2, 100)
        };

        var result = service.Detect(records);

        var anomaly = result.Single(r => r.Energy == 100);

        Assert.False(anomaly.TemperatureAnomaly);
        Assert.False(anomaly.PressureAnomaly);
        Assert.False(anomaly.VibrationAnomaly);
        Assert.True(anomaly.EnergyAnomaly);
        Assert.True(anomaly.IsAnomaly);
    }

    [Fact]
    public void Anomaly_detection_preserves_record_identity()
    {
        var service = new AnomalyDetectionService();

        var timestamp = new DateTime(
            2026,
            1,
            1,
            12,
            0,
            0,
            DateTimeKind.Utc);

        var records = new[]
        {
            new TelemetryRecord
            {
                TelemetryRecordId = 123,
                MachineId = 7,
                Timestamp = timestamp,
                Temperature = 50,
                Pressure = 5,
                Vibration = 2,
                Energy = 50
            }
        };

        var result = Assert.Single(service.Detect(records));

        Assert.Equal(123, result.TelemetryRecordId);
        Assert.Equal(7, result.MachineId);
        Assert.Equal(timestamp, result.Timestamp);
        Assert.Equal(50, result.Temperature);
        Assert.Equal(5, result.Pressure);
        Assert.Equal(2, result.Vibration);
        Assert.Equal(50, result.Energy);
    }

    private static TelemetryRecord CreateRecord(
        int machineId,
        float temperature,
        float pressure,
        float vibration,
        float energy)
    {
        return new TelemetryRecord
        {
            MachineId = machineId,
            Timestamp = DateTime.UtcNow,
            Temperature = temperature,
            Pressure = pressure,
            Vibration = vibration,
            Energy = energy
        };
    }
}