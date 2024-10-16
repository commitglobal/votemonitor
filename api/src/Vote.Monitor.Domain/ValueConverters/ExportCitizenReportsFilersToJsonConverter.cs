using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

namespace Vote.Monitor.Domain.ValueConverters;

public class ExportCitizenReportsFilersToJsonConverter() : ValueConverter<ExportCitizenReportsFilers, string>(
    v => ExportCitizenReportsFilersSerialize(v),
    s => ExportCitizenReportsFilersDeserialize(s))
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };


    public static string ExportCitizenReportsFilersSerialize(ExportCitizenReportsFilers? value)
    {
        if (value is null)
        {
            return "";
        }

        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static ExportCitizenReportsFilers ExportCitizenReportsFilersDeserialize(string value)
    {
        ExportCitizenReportsFilers? parseResult = null;
        try
        {
            parseResult = JsonSerializer.Deserialize<ExportCitizenReportsFilers>(value);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to deserialize citizen reports filters: {message}", e.Message);
        }

        return parseResult ?? new ExportCitizenReportsFilers();
    }
}