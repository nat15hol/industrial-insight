namespace server.DTOs;

public class TelemetryRecordResponse
{
    public int TelemetryRecordId { get; set; }
    public DateTime Timestamp { get; set; }
    public float Temperature { get; set; }
    public float Pressure { get; set; }
    public float Vibration { get; set; }
    public float Energy { get; set; }
    public int MachineId { get; set; }
}