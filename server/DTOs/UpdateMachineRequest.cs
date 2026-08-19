namespace server.DTOs;

public class UpdateMachineRequest
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public float Runtime { get; set; }
    public int LocationId { get; set; }
}
