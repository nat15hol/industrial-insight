public interface IIncidentAiService {
Task<IncidentAiSuggestionDto?> SuggestAsync(string description, string machineContext); 
}