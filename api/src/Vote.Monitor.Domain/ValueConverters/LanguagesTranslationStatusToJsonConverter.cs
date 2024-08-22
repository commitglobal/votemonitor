using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.ValueConverters;

public class LanguagesTranslationStatusToJsonConverter : ValueConverter<LanguagesTranslationStatus, string>
{
    public LanguagesTranslationStatusToJsonConverter() : base(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true }),
        v => JsonSerializer.Deserialize<LanguagesTranslationStatus>(v, new JsonSerializerOptions { WriteIndented = true }))
    {
    }
}