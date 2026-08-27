using System.ComponentModel.DataAnnotations;

namespace server.DTOs;

public class CreateMaintenanceTaskRequest
{
    [Required]
    [RegularExpression("^(Pending|InProgress|Completed)$")]
    public string Status { get; set; } = "Pending";

    [Range(1, int.MaxValue)]
    public int IncidentId { get; set; }

    [Range(1, int.MaxValue)]
    public int AssignedToUserId { get; set; }
}
