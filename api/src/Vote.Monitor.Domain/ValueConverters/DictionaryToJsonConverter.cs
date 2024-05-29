using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Vote.Monitor.Domain.ValueConverters;

public class DictionaryToJsonConverter : ValueConverter<Dictionary<string, string>, string>
{
    public DictionaryToJsonConverter() : base(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true }),
        v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions { WriteIndented = true }))
    {
    }
}
