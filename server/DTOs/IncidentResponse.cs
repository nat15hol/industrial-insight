namespace server.DTOs;

public class IncidentResponse
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
    public string MachineName { get; set; } = string.Empty;
    public int ReportedByUserId { get; set; }
}