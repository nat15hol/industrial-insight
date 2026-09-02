using server.Models;

namespace server.Services;

public class PriorityScoreService
{
    public PriorityScoreResult Calculate(IEnumerable<Incident> incidents)
    {
        var openIncidents = incidents
            .Where(i => i.Status == "Open")
            .ToList();

        var open = openIncidents.Count;

        var hasCritical = openIncidents.Any(i =>
            string.Equals(i.Priority, "High", StringComparison.OrdinalIgnoreCase));

        var recurringIssue = openIncidents
            .GroupBy(i => i.Category, StringComparer.OrdinalIgnoreCase)
            .Any(group => group.Count() >= 2);

        var score =
            40f * Math.Min(open / 5f, 1f)
            + (hasCritical ? 40f : 0f)
            + (recurringIssue ? 20f : 0f);

        var bucket = score >= 70
            ? "HIGH"
            : score >= 40
                ? "MEDIUM"
                : "LOW";

        return new PriorityScoreResult
        {
            Score = score,
            Bucket = bucket
        };
    }
}

public class PriorityScoreResult
{
    public float Score { get; set; }
    public string Bucket { get; set; } = string.Empty;
}