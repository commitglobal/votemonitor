using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

namespace Vote.Monitor.Domain.ValueConverters;

public class ExportFormSubmissionsFiltersToJsonConverter() : ValueConverter<ExportFormSubmissionsFilters, string>(
    v => ExportFormSubmissionsFiltersSerialize(v),
    s => ExportFormSubmissionsFiltersDeserialize(s))
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public static string ExportFormSubmissionsFiltersSerialize(ExportFormSubmissionsFilters? value)
    {
        if (value is null)
        {
            return "";
        }

        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static ExportFormSubmissionsFilters ExportFormSubmissionsFiltersDeserialize(string value)
    {
        ExportFormSubmissionsFilters? parseResult = null;
        try
        {
            parseResult = JsonSerializer.Deserialize<ExportFormSubmissionsFilters>(value);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to deserialize form submissions filters: {message}", e.Message);
        }

        return parseResult ?? new ExportFormSubmissionsFilters();
    }
}