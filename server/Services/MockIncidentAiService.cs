using server.DTOs;

namespace server.Services;
    public class MockIncidentAiService : IIncidentAiService
    {
        public Task<IncidentAiSuggestionDto?> SuggestAsync(
        string description,
        string machineContext)
    {
        var text = description.ToLowerInvariant();

        var category = "Other";
        var priority = "Medium";
        var recommendedAction =
            "Inspect the machine and investigate the reported issue.";

        if (text.Contains("oil") || text.Contains("leak"))
        {
            category = "Mechanical";
            priority = "High";
            recommendedAction = "Inspect the machine for oil leakage and mechanical damage.";
        }
        else if (text.Contains("electrical") || text.Contains("power"))
        {
            category = "Electrical";
            priority = "High";
            recommendedAction = "Inspect the electrical system and power connections.";
        }
        else if (text.Contains("plc") || text.Contains("automation"))
        {
            category = "Automation";
            priority = "Medium";
            recommendedAction = "Inspect the PLC and automation system for faults.";
        }
        else if (text.Contains("software") || text.Contains("error"))
        {
            category = "Software";
            priority = "Medium";
            recommendedAction = "Investigate the software error and review relevant logs.";
        }
        else if (text.Contains("hydraulic") || text.Contains("pressure"))
        {
            category = "Hydraulics";
            priority = "High";
            recommendedAction = "Inspect the hydraulic system and check pressure levels.";
        }

        return Task.FromResult<IncidentAiSuggestionDto?>(
            new IncidentAiSuggestionDto
            {
                Category = category,
                Priority = priority,
                RecommendedAction = recommendedAction
            });
    }
}