using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

namespace Vote.Monitor.Domain.ValueConverters;

public class ExportQuickReportsFiltersToJsonConverter() : ValueConverter<ExportQuickReportsFilters, string>(
    v => ExportQuickReportsFiltersSerialize(v),
    s => ExportQuickReportsFiltersDeserialize(s))
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public static string ExportQuickReportsFiltersSerialize(ExportQuickReportsFilters? value)
    {
        if (value is null)
        {
            return "";
        }

        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static ExportQuickReportsFilters ExportQuickReportsFiltersDeserialize(string value)
    {
        ExportQuickReportsFilters? parseResult = null;
        try
        {
            parseResult = JsonSerializer.Deserialize<ExportQuickReportsFilters>(value);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to deserialize quick reports filters: {message}", e.Message);
        }

        return parseResult ?? new ExportQuickReportsFilters();
    }
}