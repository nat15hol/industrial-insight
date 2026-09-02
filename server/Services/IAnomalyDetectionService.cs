using server.DTOs;
using server.Models;

namespace server.Services;

public interface IAnomalyDetectionService
{
    IReadOnlyList<AnomalyDetectionResponse> Detect(
        IEnumerable<TelemetryRecord> records);
}