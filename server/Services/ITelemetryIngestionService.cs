using server.DTOs;

namespace server.Services;

public interface ITelemetryIngestionService
{
    Task<TelemetryIngestionResult> IngestAsync(Stream csvStream);
}