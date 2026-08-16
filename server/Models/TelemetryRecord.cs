namespace server.Models;

public class TelemetryRecord
{
    public int TelemetryId { get; set; }
    public DateTime Timestamp { get; set; }
    public float Temperature { get; set; }
    public float Pressure { get; set; }
    public float Vibration { get; set; }
    public float Energy { get; set; }

    public int MachineId { get; set; }
    public Machine? Machine { get; set; }
}
