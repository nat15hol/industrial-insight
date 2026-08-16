namespace server.Models;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public ICollection<Incident> ReportedIncidents { get; set; } = new List<Incident>();
    public ICollection<MaintenanceTask> AssignedTasks { get; set; } = new List<MaintenanceTask>();
}
