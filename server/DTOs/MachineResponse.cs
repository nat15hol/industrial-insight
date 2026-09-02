namespace server.DTOs;

public class MachineResponse
{
    public int MachineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public float Runtime { get; set; }
    public int LocationId { get; set; }
    public LocationResponse? Location { get; set; }

    public float PriorityScore { get; set; }
    public string PriorityBucket { get; set; } = string.Empty;
}

public class LocationResponse
{
    public int LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
