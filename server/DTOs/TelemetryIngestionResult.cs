namespace server.DTOs;

public class TelemetryIngestionResult
{
    public int PipelineRunId { get; set; }
    public int RecordsProcessed { get; set; }
    public int RecordsAccepted { get; set; }
    public int RecordsRejected { get; set; }
    public int Duplicates { get; set; }
    public float DataQualityPct { get; set; }
    public string Status { get; set; } = string.Empty;
}