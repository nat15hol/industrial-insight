namespace server.Models;

public class Incident
{
    public int IncidentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? AiSuggestion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public int MachineId { get; set; }
    public Machine? Machine { get; set; }

    public int ReportedByUserId { get; set; }
    public User? ReportedByUser { get; set; }

    public ICollection<MaintenanceTask> MaintenanceTasks { get; set; } = new List<MaintenanceTask>();
}
