using System.ComponentModel.DataAnnotations;

namespace server.DTOs;

public class UpdateIncidentRequest
{
    [Required]
    [RegularExpression("^(Open|Closed)$")]
    public string Status { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    public string? AiSuggestion { get; set; }

    public DateTime? ResolvedAt { get; set; }
}
