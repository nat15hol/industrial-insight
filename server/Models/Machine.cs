namespace server.Models;

public class Machine
{
    public int MachineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public float Runtime { get; set; }

    public int LocationId { get; set; }
    public Location? Location { get; set; }

    public ICollection<TelemetryRecord> TelemetryRecords { get; set; } = new List<TelemetryRecord>();
    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
}
