namespace server.DTOs;

public class MaintenanceTaskResponse
{
    public int MaintenanceTaskId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int IncidentId { get; set; }
    public int AssignedToUserId { get; set; }
}
