using System.Text.Json;

namespace Vote.Monitor.Core.Helpers;

public static class ObjectUtils
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public static T DeepClone<T>(this T obj)
    {
        var json = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
        return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions)!;
    }
}
