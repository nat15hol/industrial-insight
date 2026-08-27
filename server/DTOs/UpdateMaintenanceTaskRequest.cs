using System.ComponentModel.DataAnnotations;

namespace server.DTOs;

public class UpdateMaintenanceTaskRequest
{
    [Required]
    [RegularExpression("^(Pending|InProgress|Completed)$")]
    public string Status { get; set; } = string.Empty;

    public DateTime? CompletedAt { get; set; }
}
