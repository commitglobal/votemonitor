using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.ValueConverters;

public class TranslatedStringToJsonConverter : ValueConverter<TranslatedString, string>
{
    public TranslatedStringToJsonConverter() : base(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true }),
        v => JsonSerializer.Deserialize<TranslatedString>(v, new JsonSerializerOptions { WriteIndented = true }))
    {
    }
}