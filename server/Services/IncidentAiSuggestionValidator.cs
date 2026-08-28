using server.DTOs;

namespace server.Services;

public class IncidentAiSuggestionValidator
{
    public bool IsValid(IncidentAiSuggestionDto suggestion)
    {
        var validCategories = new[]
        {
            "Mechanical",
            "Electrical",
            "Automation",
            "Software",
            "Hydraulics",
            "Other"
        };

        var validPriorities = new[]
        {
            "High",
            "Medium",
            "Low"
        };

        if (!validCategories.Contains(suggestion.Category))
        {
            return false;
        }

        if (!validPriorities.Contains(suggestion.Priority))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(suggestion.RecommendedAction))
        {
            return false;
        }

        if (suggestion.RecommendedAction.Length > 1000)
        {
            return false;
        }

        return true;
    }
}
