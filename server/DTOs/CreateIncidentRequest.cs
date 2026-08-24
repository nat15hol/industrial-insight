namespace server.DTOs;

public class CreateIncidentRequest
{
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? AiSuggestion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int MachineId { get; set; }
    public int ReportedByUserId { get; set; }
}