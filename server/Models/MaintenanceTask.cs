namespace server.Models;

public class MaintenanceTask
{
    public int MaintenanceTaskId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public int IncidentId { get; set; }
    public Incident? Incident { get; set; }

    public int AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }
}
