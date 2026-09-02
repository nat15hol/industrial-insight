namespace server.DTOs;

public class AnomalyDetectionResponse
{
    public int TelemetryRecordId { get; set; }
    public int MachineId { get; set; }
    public DateTime Timestamp { get; set; }

    public float Temperature { get; set; }
    public float Pressure { get; set; }
    public float Vibration { get; set; }
    public float Energy { get; set; }

    public bool TemperatureAnomaly { get; set; }
    public bool PressureAnomaly { get; set; }
    public bool VibrationAnomaly { get; set; }
    public bool EnergyAnomaly { get; set; }

    public bool IsAnomaly =>
        TemperatureAnomaly
        || PressureAnomaly
        || VibrationAnomaly
        || EnergyAnomaly;
}