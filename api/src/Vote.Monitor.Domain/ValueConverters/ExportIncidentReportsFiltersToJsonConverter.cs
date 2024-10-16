using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

namespace Vote.Monitor.Domain.ValueConverters;

public class ExportIncidentReportsFiltersToJsonConverter() : ValueConverter<ExportIncidentReportsFilters, string>(
    v => ExportIncidentReportsFiltersSerialize(v),
    s => ExportIncidentReportsFiltersDeserialize(s))
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public static string ExportIncidentReportsFiltersSerialize(ExportIncidentReportsFilters? value)
    {
        if (value is null)
        {
            return "";
        }

        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static ExportIncidentReportsFilters ExportIncidentReportsFiltersDeserialize(string value)
    {
        ExportIncidentReportsFilters? parseResult = null;
        try
        {
            parseResult = JsonSerializer.Deserialize<ExportIncidentReportsFilters>(value);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to deserialize incident reports filters: {message}", e.Message);

        }

        return parseResult ?? new ExportIncidentReportsFilters();
    }
}