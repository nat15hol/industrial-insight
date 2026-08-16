namespace server.Models;

public class PipelineRun
{
    public int RunId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int RecordsProcessed { get; set; }
    public int RecordsAccepted { get; set; }
    public int RecordsRejected { get; set; }
    public int Duplicates { get; set; }
    public float DataQualityPct { get; set; }
    public string Status { get; set; } = string.Empty;
}
