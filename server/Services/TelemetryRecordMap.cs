using CsvHelper.Configuration;
using server.Models;

namespace server.Services;

public sealed class TelemetryRecordMap : ClassMap<TelemetryRecord>
{
    public TelemetryRecordMap()
    {
        Map(m => m.MachineId).Name("MachineId");
        Map(m => m.Timestamp).Name("Timestamp");
        Map(m => m.Temperature).Name("Temperature");
        Map(m => m.Pressure).Name("Pressure");
        Map(m => m.Vibration).Name("Vibration");
        Map(m => m.Energy).Name("Energy");
    }
}